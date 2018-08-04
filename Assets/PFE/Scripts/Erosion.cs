using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Erosion{

	public static float[,] thermalErosion(float[,] heightMap, int cycle,float limitCurve,float percent,int width)
	{
		float[,] voisinLocal = new float[3, 3];
		float[,] matiere = new float[width, width];
		for (int k = 0; k < cycle; k++) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
                    matiere[x, y] = 0.0f;
				}
			}
			for (int x = 1; x < width-1; x++) {
				for (int y = 1; y < width-1; y++) {
                    float diffMax = 0.0f;
                    float diffTotal = 0.0f;
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float diffTmp=heightMap[x,y]-heightMap[x+i,y+j];
                            voisinLocal[1+i,1+j]=diffTmp;
							if (diffTmp > limitCurve) {
                                diffTotal += diffTmp;
								if (diffTmp > diffMax) {
                                    diffMax = diffTmp;
								}
							}
						}
					}
					float tmp = 0.0f;
					float value = 0.0f;
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float diffTmp = voisinLocal[1+i,1+j];
							if (diffTmp>limitCurve) {
								tmp = percent * (diffMax - limitCurve) * (diffTmp / diffTotal);
                                matiere[x + i, y + j] += tmp;
								value += tmp;
							}
						}
					}
                    matiere[x, y]-=value;
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
					heightMap [x, y] += matiere[x, y];
				}
			}
		}
		return heightMap;
	}

    
	public static float[,] hydraulicErosion(float[,] heightMap, int cycle,float amountRain,float solubility,float evaporation,int width){
		float[,] voisinLocal = new float[3, 3];
		float[,] matiere = new float[width, width];
		float[,] eauPluie = new float[width, width];
		float[,] matiereDiff = new float[width, width];
		float[,] eauDiff = new float[width, width];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
                eauPluie[x,y]=0.0f;
			}
		}
		for (int k = 0; k < cycle; k++) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
                    eauPluie[x, y]+=amountRain;
				}
			}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
					float materialTmp = Mathf.Min(solubility * eauPluie[x,y], heightMap[x, y]);
					heightMap[x, y] -= materialTmp;
                    matiere[x,y] += materialTmp;
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
                    matiereDiff[x, y] = 0.0f;
                    eauDiff[x,y]=0.0f;
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
					float d_total = 0.0f;
					float a_total = 0.0f;
					float altitude = heightMap [x, y] + eauPluie[x, y];
					int n = 0;
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float altitudeLocal=heightMap [x+i, y+j] + eauPluie[x+i, y+j];
							float diffTmp = altitude - altitudeLocal;
                            voisinLocal[1 + i, 1 + j] = diffTmp;
							if (diffTmp > 0.0) {
								d_total += diffTmp;
								a_total += altitudeLocal;
								n++;
							}
						}
					}
					if (n == 0) {
						continue;
					}
					float altitudeAvg = a_total / n;
					float diffAvg = Mathf.Min(eauPluie[x, y],altitude-altitudeAvg);
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float diffTmp = voisinLocal[1 + i, 1 + j];
							if (diffTmp > 0.0) {
								float dw = diffAvg * (diffTmp / d_total);
                                eauDiff[x + i, y + j] += dw;
                                eauDiff[x, y] -= dw;

								float dm = eauDiff[x, y] * (dw / eauPluie[x, y]);
                                matiereDiff[x + i, y + j] += dm;
                                matiereDiff[x, y] -= dm;
							}
						}
					}
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
                    matiere[x,y] += matiereDiff[x,y];
                    eauPluie[x,y]+=eauDiff[x,y];
				}
			}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
					float waterResult = eauPluie[x, y] * (evaporation);
                    eauPluie[x, y] -= waterResult;
					float material_max = solubility * waterResult;
					float dm = Mathf.Min (matiere[x, y], material_max);
                    matiere[x, y] -= dm;
					heightMap [x, y] += dm;
				}
			}
		}
        return heightMap;
	}

	public static float[,] fastErosion(float[,] heightMap, int cycle,float limitCurve,float percent,int width){
		float[,] matiere = new float[width, width];
		for (int k = 0; k < cycle; k++) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
                    matiere[x, y] = 0.0f;
				}
			}
			for (int x = 1; x < width-1; x++) {
				for (int y = 1; y < width-1; y++) {
					float altitudemax = 0.0f;
					Vector2Int position = new Vector2Int (x, y);
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float altitudeDiff=heightMap[x,y]-heightMap[x+i,y+j];
							if (altitudeDiff > altitudemax) {
								altitudemax = altitudeDiff;
								position=new Vector2Int (x+i, y+j);
							}
						}
					}
					if (0 < altitudemax & altitudemax <= limitCurve) {
                        matiere[x, y] -= percent*altitudemax;
                        matiere[position.x, position.y] += percent*altitudemax;
					}
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
					heightMap [x, y] += matiere[x, y];
				}
			}
		}
		return heightMap;
	}

    /*
     * Ancienne version, à voir si utile à garder ?
	public static float[,] hydraulicErosion(float[,] heightMap, int cycle,float amountRain,float solubility,float evaporation,int width){
		float[,] diff = new float[3, 3];
		float[,] water = new float[width, width];
		float[,] waterDiff = new float[width, width];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < width; y++) {
                water[x,y]=0.0f;
			}
		}
		for (int k = 0; k < cycle; k++) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
                    water[x, y]+=amountRain;
                    heightMap[x, y] -= amountRain * solubility;
                }
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
                    waterDiff[x,y]=0.0f;
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
					float d_total = 0.0f;
					float a_total = 0.0f;
					float altitude = heightMap [x, y] + water [x, y];
					int n = 0;
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float altitudeLocal=heightMap [x+i, y+j] + water[x+i, y+j];
							float diffTmp = altitude - altitudeLocal;
							diff [1 + i, 1 + j] = diffTmp;
							if (diffTmp > 0.0) {
								d_total += diffTmp;
								a_total += altitudeLocal;
								n++;
							}
						}
					}
					if (n == 0) {
						continue;
					}
					float altitudeAvg = a_total / n;
					float diffAvg = Mathf.Min(water[x, y],altitude-altitudeAvg);
					for (int i = -1; i <= 1; i++) {
						for (int j = -1; j <= 1; j++) {
							float diffTmp = diff [1 + i, 1 + j];
							if (diffTmp > 0.0) {
								float dw = diffAvg * (diffTmp / d_total);
                                waterDiff[x + i, y + j] += dw;
                                waterDiff[x, y] -= dw;
							}
						}
					}
				}
			}
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < width - 1; y++) {
                    water[x,y]+=waterDiff[x,y];
				}
			}
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < width; y++) {
					float waterResult = water[x, y] * (evaporation);
                    water[x, y] -= waterResult;
					float material = solubility * waterResult;
					float dm =material;
					heightMap [x, y] += dm;
				}
			}
		}
        
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < width - 1; y++)
            {
                heightMap[x, y] += solubility * water[x, y];
            }
        }
        return heightMap;
	}*/
}