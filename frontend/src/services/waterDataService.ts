import axios from 'axios'

export interface WaterStation {
  stationId: string
  stationName: string
  province: string
  latitude: number
  longitude: number
  status: string
  lastUpdated: string
}

export interface WaterStationData {
  stationId: string
  parameter: string
  parameterName: string
  value: number
  unit: string
  timestamp: string
}

export interface StationDetailResponse {
  station: WaterStation
  recentData: WaterStationData[]
  currentWaterLevel: number
  currentDischarge: number
  waterLevelStatus: string
  dischargeStatus: string
}

const API_BASE_URL = 'http://localhost:5000/api'

class WaterDataService {
  private axiosInstance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json'
    }
  })

  async getWaterStations(): Promise<WaterStation[]> {
    try {
      const response = await this.axiosInstance.get<WaterStation[]>('/water/stations')
      console.log('Water stations API response:', response.data)
      return response.data
    } catch (error: any) {
      console.error('Error fetching water stations:', error)
      if (error.response) {
        console.error('API Error Response:', error.response.data)
        console.error('API Error Status:', error.response.status)
      } else if (error.request) {
        console.error('No response received:', error.request)
      } else {
        console.error('Error setting up request:', error.message)
      }
      throw new Error('Failed to fetch water stations')
    }
  }

  async getStationData(stationId: string): Promise<StationDetailResponse> {
    try {
      const response = await this.axiosInstance.get<StationDetailResponse>(`/water/station/${stationId}`)
      console.log(`Station ${stationId} data:`, response.data)
      return response.data
    } catch (error: any) {
      console.error(`Error fetching data for station ${stationId}:`, error)
      if (error.response) {
        console.error('API Error Response:', error.response.data)
        console.error('API Error Status:', error.response.status)
      }
      throw new Error(`Failed to fetch data for station ${stationId}`)
    }
  }
}

export const waterDataService = new WaterDataService()