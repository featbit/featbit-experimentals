export interface EnvironmentDto {
  id: string;
  name: string | null;
  key: string | null;
}

export interface ProjectDto {
  id: string;
  name: string | null;
  key: string | null;
  environments: EnvironmentDto[];
}

export interface OrganizationDto {
  id: string;
  name: string | null;
  key: string | null;
  projects: ProjectDto[];
}

export interface WorkspaceDto {
  id: string;
  name: string | null;
  key: string | null;
  organizations: OrganizationDto[];
}

export interface WorkspaceHierarchyResponse {
  workspaces: WorkspaceDto[];
}

export interface DeleteEventsResponse {
  deletedCount: number;
  message: string;
}

export interface PreviewDeleteResponse {
  eventsToDelete: number;
  message: string;
}

export interface EventsSummary {
  totalCount: number;
  flagValueCount: number;
  customEventsCount: number;
  oldestEventDate: string | null;
  newestEventDate: string | null;
}

export type SelectionType = 'workspace' | 'organization' | 'project' | 'environment';

export interface Selection {
  type: SelectionType;
  id: string;
  name: string;
  envIds?: string[]; // For project, this contains all environment IDs
}
