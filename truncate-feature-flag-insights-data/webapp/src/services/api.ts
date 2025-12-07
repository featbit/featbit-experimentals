import type {
  WorkspaceHierarchyResponse,
  DeleteEventsResponse,
  PreviewDeleteResponse,
  EventsSummary,
} from '@/types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7031';

async function fetchApi<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Request failed' }));
    throw new Error(error.message || `HTTP ${response.status}`);
  }

  return response.json();
}

export const api = {
  // Hierarchy
  getHierarchy: () => fetchApi<WorkspaceHierarchyResponse>('/api/hierarchy'),

  // Summary
  getEventsSummary: () => fetchApi<EventsSummary>('/api/events/summary'),

  // By Timestamp
  previewDeleteByTimestamp: (params: {
    beforeDate?: string;
    afterDate?: string;
    eventType?: string;
  }) =>
    fetchApi<PreviewDeleteResponse>('/api/events/by-timestamp/preview', {
      method: 'POST',
      body: JSON.stringify(params),
    }),

  deleteByTimestamp: (params: {
    beforeDate?: string;
    afterDate?: string;
    eventType?: string;
  }) =>
    fetchApi<DeleteEventsResponse>('/api/events/by-timestamp', {
      method: 'DELETE',
      body: JSON.stringify(params),
    }),

  // By Environment & Timestamp
  previewDeleteByEnvTimestamp: (params: {
    envId: string;
    beforeDate?: string;
    afterDate?: string;
    eventType?: string;
  }) =>
    fetchApi<PreviewDeleteResponse>('/api/events/by-env-timestamp/preview', {
      method: 'POST',
      body: JSON.stringify(params),
    }),

  deleteByEnvTimestamp: (params: {
    envId: string;
    beforeDate?: string;
    afterDate?: string;
    eventType?: string;
  }) =>
    fetchApi<DeleteEventsResponse>('/api/events/by-env-timestamp', {
      method: 'DELETE',
      body: JSON.stringify(params),
    }),

  // By Environment & Feature Flag Key
  previewDeleteByEnvFlagKey: (params: {
    envId: string;
    featureFlagKey: string;
    beforeDate?: string;
    afterDate?: string;
  }) =>
    fetchApi<PreviewDeleteResponse>('/api/events/by-env-flagkey/preview', {
      method: 'POST',
      body: JSON.stringify(params),
    }),

  deleteByEnvFlagKey: (params: {
    envId: string;
    featureFlagKey: string;
    beforeDate?: string;
    afterDate?: string;
  }) =>
    fetchApi<DeleteEventsResponse>('/api/events/by-env-flagkey', {
      method: 'DELETE',
      body: JSON.stringify(params),
    }),
};
