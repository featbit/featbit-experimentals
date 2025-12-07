import { useState } from 'react';
import { ChevronRight, ChevronDown, Building2, FolderKanban, Layers, Globe } from 'lucide-react';
import { cn } from '@/lib/utils';
import type {
  WorkspaceDto,
  OrganizationDto,
  ProjectDto,
  EnvironmentDto,
  Selection,
} from '@/types';

interface HierarchyTreeProps {
  workspaces: WorkspaceDto[];
  selectedId: string | null;
  onSelect: (selection: Selection) => void;
}

export function HierarchyTree({ workspaces, selectedId, onSelect }: HierarchyTreeProps) {
  if (workspaces.length === 0) {
    return (
      <div className="flex items-center justify-center h-full text-muted-foreground p-4">
        No data found
      </div>
    );
  }

  return (
    <div className="p-2">
      {workspaces.map((workspace) => (
        <WorkspaceNode
          key={workspace.id}
          workspace={workspace}
          selectedId={selectedId}
          onSelect={onSelect}
        />
      ))}
    </div>
  );
}

interface WorkspaceNodeProps {
  workspace: WorkspaceDto;
  selectedId: string | null;
  onSelect: (selection: Selection) => void;
}

function WorkspaceNode({ workspace, selectedId, onSelect }: WorkspaceNodeProps) {
  const [expanded, setExpanded] = useState(true);
  const isSelected = selectedId === workspace.id;

  const allEnvIds = workspace.organizations.flatMap((org) =>
    org.projects.flatMap((proj) => proj.environments.map((env) => env.id))
  );

  return (
    <div>
      <div
        className={cn(
          'flex items-center gap-1 px-2 py-1.5 rounded-md cursor-pointer hover:bg-accent',
          isSelected && 'bg-accent'
        )}
        onClick={() =>
          onSelect({
            type: 'workspace',
            id: workspace.id,
            name: workspace.name || 'Unnamed Workspace',
            envIds: allEnvIds,
          })
        }
      >
        <button
          className="p-0.5 hover:bg-muted rounded"
          onClick={(e) => {
            e.stopPropagation();
            setExpanded(!expanded);
          }}
        >
          {expanded ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
        </button>
        <Building2 className="h-4 w-4 text-blue-500" />
        <span className="text-sm font-medium truncate">{workspace.name || 'Unnamed'}</span>
      </div>

      {expanded && (
        <div className="ml-4">
          {workspace.organizations.map((org) => (
            <OrganizationNode
              key={org.id}
              organization={org}
              selectedId={selectedId}
              onSelect={onSelect}
            />
          ))}
        </div>
      )}
    </div>
  );
}

interface OrganizationNodeProps {
  organization: OrganizationDto;
  selectedId: string | null;
  onSelect: (selection: Selection) => void;
}

function OrganizationNode({ organization, selectedId, onSelect }: OrganizationNodeProps) {
  const [expanded, setExpanded] = useState(true);
  const isSelected = selectedId === organization.id;

  const allEnvIds = organization.projects.flatMap((proj) =>
    proj.environments.map((env) => env.id)
  );

  return (
    <div>
      <div
        className={cn(
          'flex items-center gap-1 px-2 py-1.5 rounded-md cursor-pointer hover:bg-accent',
          isSelected && 'bg-accent'
        )}
        onClick={() =>
          onSelect({
            type: 'organization',
            id: organization.id,
            name: organization.name || 'Unnamed Organization',
            envIds: allEnvIds,
          })
        }
      >
        <button
          className="p-0.5 hover:bg-muted rounded"
          onClick={(e) => {
            e.stopPropagation();
            setExpanded(!expanded);
          }}
        >
          {expanded ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
        </button>
        <Globe className="h-4 w-4 text-purple-500" />
        <span className="text-sm truncate">{organization.name || 'Unnamed'}</span>
      </div>

      {expanded && (
        <div className="ml-4">
          {organization.projects.map((project) => (
            <ProjectNode
              key={project.id}
              project={project}
              selectedId={selectedId}
              onSelect={onSelect}
            />
          ))}
        </div>
      )}
    </div>
  );
}

interface ProjectNodeProps {
  project: ProjectDto;
  selectedId: string | null;
  onSelect: (selection: Selection) => void;
}

function ProjectNode({ project, selectedId, onSelect }: ProjectNodeProps) {
  const [expanded, setExpanded] = useState(false);
  const isSelected = selectedId === project.id;

  const envIds = project.environments.map((env) => env.id);

  return (
    <div>
      <div
        className={cn(
          'flex items-center gap-1 px-2 py-1.5 rounded-md cursor-pointer hover:bg-accent',
          isSelected && 'bg-accent'
        )}
        onClick={() =>
          onSelect({
            type: 'project',
            id: project.id,
            name: project.name || 'Unnamed Project',
            envIds,
          })
        }
      >
        <button
          className="p-0.5 hover:bg-muted rounded"
          onClick={(e) => {
            e.stopPropagation();
            setExpanded(!expanded);
          }}
        >
          {expanded ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
        </button>
        <FolderKanban className="h-4 w-4 text-orange-500" />
        <span className="text-sm truncate">{project.name || 'Unnamed'}</span>
        <span className="text-xs text-muted-foreground ml-auto">
          {project.environments.length} env
        </span>
      </div>

      {expanded && (
        <div className="ml-4">
          {project.environments.map((env) => (
            <EnvironmentNode
              key={env.id}
              environment={env}
              selectedId={selectedId}
              onSelect={onSelect}
            />
          ))}
        </div>
      )}
    </div>
  );
}

interface EnvironmentNodeProps {
  environment: EnvironmentDto;
  selectedId: string | null;
  onSelect: (selection: Selection) => void;
}

function EnvironmentNode({ environment, selectedId, onSelect }: EnvironmentNodeProps) {
  const isSelected = selectedId === environment.id;

  return (
    <div
      className={cn(
        'flex items-center gap-1 px-2 py-1.5 rounded-md cursor-pointer hover:bg-accent ml-5',
        isSelected && 'bg-accent'
      )}
      onClick={() =>
        onSelect({
          type: 'environment',
          id: environment.id,
          name: environment.name || 'Unnamed Environment',
        })
      }
    >
      <Layers className="h-4 w-4 text-green-500" />
      <span className="text-sm truncate">{environment.name || 'Unnamed'}</span>
    </div>
  );
}
