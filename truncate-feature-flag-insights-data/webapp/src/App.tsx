import { useState, useEffect } from 'react';
import { HierarchyTree } from '@/components/HierarchyTree';
import { DeletePanel } from '@/components/DeletePanel';
import { ScrollArea } from '@/components/ui/scroll-area';
import { api } from '@/services/api';
import type { WorkspaceDto, Selection } from '@/types';

function App() {
  const [workspaces, setWorkspaces] = useState<WorkspaceDto[]>([]);
  const [selection, setSelection] = useState<Selection | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadHierarchy();
  }, []);

  const loadHierarchy = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await api.getHierarchy();
      setWorkspaces(data.workspaces);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load hierarchy');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="h-screen flex bg-background text-foreground">
      {/* Left Panel - Hierarchy Tree */}
      <aside className="w-80 border-r flex-shrink-0 flex flex-col">
        <div className="px-6 py-4 border-b">
          <h1 className="text-2xl font-bold">Events Cleanup Tool</h1>
          <p className="text-sm text-muted-foreground">
            Delete unused events data from the database
          </p>
        </div>
        <div className="p-4 border-b">
          <h2 className="font-semibold text-sm text-muted-foreground uppercase tracking-wider">
            Resources
          </h2>
        </div>
        <div className="flex-1 overflow-y-auto">
          {loading ? (
            <div className="p-4 text-center text-muted-foreground">Loading...</div>
          ) : error ? (
            <div className="p-4">
              <p className="text-destructive text-sm">{error}</p>
              <button
                onClick={loadHierarchy}
                className="text-sm text-primary hover:underline mt-2"
              >
                Retry
              </button>
            </div>
          ) : (
            <HierarchyTree
              workspaces={workspaces}
              onSelect={setSelection}
              selectedId={selection?.id ?? null}
            />
          )}
        </div>
        </aside>

        {/* Right Panel - Delete Query Panel */}
        <main className="flex-1 overflow-hidden">
          <DeletePanel selection={selection} />
        </main>
      </div>
  );
}

export default App
