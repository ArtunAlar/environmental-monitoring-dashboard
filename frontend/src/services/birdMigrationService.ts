export interface BirdObservation {
  speciesCode: string;
  commonName: string;
  scientificName: string;
  latitude: number;
  longitude: number;
  observationDate: string;
  count: number;
  locationName: string;
  regionCode: string;
  observerId: string;
  hasMedia: boolean;
  approved: boolean;
  checklistId: string;
}

export interface BirdSpecies {
  speciesCode: string;
  commonName: string;
  scientificName: string;
  family: string;
  order: string;
}

export interface BirdMigrationResponse {
  observations: BirdObservation[];
  species: BirdSpecies[];
  startDate: string;
  endDate: string;
  regionCode: string;
  totalObservations: number;
  totalSpecies: number;
  lastUpdated: string;
}

export interface MigrationRouteData {
  speciesCode: string;
  commonName: string;
  points: ObservationPoint[];
  firstObservation: string;
  lastObservation: string;
  totalCount: number;
}

export interface ObservationPoint {
  latitude: number;
  longitude: number;
  date: string;
  count: number;
  locationName: string;
}

export interface AnimationFrame {
  timestamp: string;
  observations: BirdObservation[];
  progress: number;
}

class BirdMigrationService {
  private readonly API_BASE = 'http://localhost:5000/api/birdmigration';

  async getObservations(
    region: string = 'CA-AB',
    species?: string,
    startDate?: string,
    endDate?: string,
    maxResults: number = 200
  ): Promise<BirdMigrationResponse> {
    try {
      const params = new URLSearchParams({
        region,
        maxResults: maxResults.toString()
      });

      if (species) params.append('species', species);
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);

      const response = await fetch(`${this.API_BASE}/observations?${params}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Bird observations fetched:', data);
      return data;
    } catch (error) {
      console.error('Error fetching bird observations:', error);
      throw error;
    }
  }

  async getMigrationRoutes(
    region: string = 'CA-AB',
    species: string,
    startDate?: string,
    endDate?: string
  ): Promise<MigrationRouteData[]> {
    try {
      const params = new URLSearchParams({ region, species });
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);

      const response = await fetch(`${this.API_BASE}/migration-routes?${params}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Migration routes fetched:', data);
      return data;
    } catch (error) {
      console.error('Error fetching migration routes:', error);
      throw error;
    }
  }

  async getSpeciesInRegion(
    region: string = 'CA-AB',
    startDate?: string,
    endDate?: string
  ): Promise<BirdSpecies[]> {
    try {
      const params = new URLSearchParams({ region });
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);

      const response = await fetch(`${this.API_BASE}/species?${params}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Species list fetched:', data);
      return data;
    } catch (error) {
      console.error('Error fetching species list:', error);
      throw error;
    }
  }

  async getRecentObservations(
    region: string = 'CA-AB',
    daysBack: number = 7,
    maxResults: number = 100
  ): Promise<BirdMigrationResponse> {
    try {
      const params = new URLSearchParams({
        region,
        daysBack: daysBack.toString(),
        maxResults: maxResults.toString()
      });

      const response = await fetch(`${this.API_BASE}/recent?${params}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Recent observations fetched:', data);
      return data;
    } catch (error) {
      console.error('Error fetching recent observations:', error);
      throw error;
    }
  }

  // Animation helper methods
  createAnimationFrames(
    observations: BirdObservation[],
    animationDuration: number = 30 // seconds
  ): AnimationFrame[] {
    if (observations.length === 0) return [];

    // Sort observations by date
    const sortedObs = [...observations].sort(
      (a, b) => new Date(a.observationDate).getTime() - new Date(b.observationDate).getTime()
    );

    const startTime = new Date(sortedObs[0].observationDate).getTime();
    const endTime = new Date(sortedObs[sortedObs.length - 1].observationDate).getTime();
    const totalDuration = endTime - startTime;

    // Create frames based on time intervals
    const frames: AnimationFrame[] = [];
    const frameCount = Math.min(animationDuration * 2, sortedObs.length); // 2 fps

    for (let i = 0; i < frameCount; i++) {
      const progress = i / (frameCount - 1);
      const currentTime = startTime + (totalDuration * progress);
      
      // Get observations up to current time
      const frameObservations = sortedObs.filter(
        obs => new Date(obs.observationDate).getTime() <= currentTime
      );

      frames.push({
        timestamp: new Date(currentTime).toISOString(),
        observations: frameObservations,
        progress
      });
    }

    return frames;
  }

  // Get species that are likely to show migration patterns
  getCommonMigratorySpecies(): string[] {
    return [
      'canwar', // Canada Warbler
      'amerobin', // American Robin
      'whiwoo', // White-throated Sparrow
      'swaspa', // Swamp Sparrow
      'yelwar', // Yellow Warbler
      'mallar', // Mallard
      'cangoo', // Canada Goose
      'snogoo', // Snow Goose
      'tundswa', // Tundra Swan
      'wooduc' // Wood Duck
    ];
  }
}

export const birdMigrationService = new BirdMigrationService();
export default birdMigrationService;