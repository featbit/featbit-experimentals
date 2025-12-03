import { useState, useEffect } from 'react';
import { Trash2, Eye, AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { generateDeleteSql } from '@/utils/sql-generator';
import { api } from '@/services/api';
import type { Selection } from '@/types';

interface DeletePanelProps {
  selection: Selection | null;
}

export function DeletePanel({ selection }: DeletePanelProps) {
  const [beforeDate, setBeforeDate] = useState('');
  const [afterDate, setAfterDate] = useState('');
  const [featureFlagKey, setFeatureFlagKey] = useState('');
  const [eventType, setEventType] = useState('FlagValue');
  const [previewCount, setPreviewCount] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<{ success: boolean; message: string } | null>(null);

  // Reset form when selection changes
  useEffect(() => {
    setPreviewCount(null);
    setResult(null);
  }, [selection]);

  const canUseFeatureFlagKey = selection?.type === 'project' || selection?.type === 'environment';

  const sqlQuery = generateDeleteSql({
    selection,
    beforeDate: beforeDate || undefined,
    afterDate: afterDate || undefined,
    featureFlagKey: canUseFeatureFlagKey ? featureFlagKey || undefined : undefined,
    eventType: eventType || undefined,
  });

  const handlePreview = async () => {
    if (!selection) return;

    setLoading(true);
    setResult(null);

    try {
      let response;

      if (featureFlagKey && canUseFeatureFlagKey) {
        // By feature flag key (only for environment)
        if (selection.type === 'environment') {
          response = await api.previewDeleteByEnvFlagKey({
            envId: selection.id,
            featureFlagKey,
          });
        } else if (selection.type === 'project' && selection.envIds) {
          // For project, we need to call preview for each environment
          let total = 0;
          for (const envId of selection.envIds) {
            const r = await api.previewDeleteByEnvFlagKey({
              envId,
              featureFlagKey,
            });
            total += r.eventsToDelete;
          }
          response = { eventsToDelete: total };
        }
      } else if (selection.type === 'environment') {
        // By environment + timestamp
        response = await api.previewDeleteByEnvTimestamp({
          envId: selection.id,
          beforeDate: beforeDate || undefined,
          afterDate: afterDate || undefined,
          eventType: eventType || undefined,
        });
      } else {
        // By timestamp only (for workspace, org, project)
        response = await api.previewDeleteByTimestamp({
          beforeDate: beforeDate || undefined,
          afterDate: afterDate || undefined,
          eventType: eventType || undefined,
        });
      }

      setPreviewCount(response?.eventsToDelete ?? 0);
    } catch (error) {
      setResult({
        success: false,
        message: error instanceof Error ? error.message : 'Preview failed',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!selection) return;

    const confirmed = window.confirm(
      `Are you sure you want to delete ${previewCount ?? 'these'} events? This action cannot be undone.`
    );
    if (!confirmed) return;

    setLoading(true);
    setResult(null);

    try {
      let response;

      if (featureFlagKey && canUseFeatureFlagKey) {
        if (selection.type === 'environment') {
          response = await api.deleteByEnvFlagKey({
            envId: selection.id,
            featureFlagKey,
          });
        } else if (selection.type === 'project' && selection.envIds) {
          let total = 0;
          for (const envId of selection.envIds) {
            const r = await api.deleteByEnvFlagKey({
              envId,
              featureFlagKey,
            });
            total += r.deletedCount;
          }
          response = { deletedCount: total, message: `Deleted ${total} events` };
        }
      } else if (selection.type === 'environment') {
        response = await api.deleteByEnvTimestamp({
          envId: selection.id,
          beforeDate: beforeDate || undefined,
          afterDate: afterDate || undefined,
          eventType: eventType || undefined,
        });
      } else {
        response = await api.deleteByTimestamp({
          beforeDate: beforeDate || undefined,
          afterDate: afterDate || undefined,
          eventType: eventType || undefined,
        });
      }

      setResult({
        success: true,
        message: response?.message ?? `Deleted ${response?.deletedCount} events`,
      });
      setPreviewCount(null);
    } catch (error) {
      setResult({
        success: false,
        message: error instanceof Error ? error.message : 'Delete failed',
      });
    } finally {
      setLoading(false);
    }
  };

  if (!selection) {
    return (
      <div className="flex items-center justify-center h-full text-muted-foreground">
        <p>Select an item from the left panel to configure delete query</p>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6 overflow-auto h-full">
      <div>
        <h2 className="text-xl font-semibold">Delete Events</h2>
        <p className="text-muted-foreground">
          Selected: <span className="font-medium text-foreground">{selection.name}</span>
          <span className="text-xs ml-2 px-2 py-0.5 bg-muted rounded">{selection.type}</span>
        </p>
      </div>

      {/* Query Options */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Query Options</CardTitle>
          <CardDescription>Configure the delete query parameters</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {/* Time Range */}
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="afterDate">After Date (inclusive)</Label>
              <Input
                id="afterDate"
                type="datetime-local"
                value={afterDate}
                onChange={(e) => setAfterDate(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="beforeDate">Before Date (exclusive)</Label>
              <Input
                id="beforeDate"
                type="datetime-local"
                value={beforeDate}
                onChange={(e) => setBeforeDate(e.target.value)}
              />
            </div>
          </div>

          {/* Event Type */}
          <div className="space-y-2">
            <Label htmlFor="eventType">Event Type</Label>
            <select
              id="eventType"
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
              value={eventType}
              onChange={(e) => setEventType(e.target.value)}
            >
              <option value="">All Events</option>
              <option value="FlagValue">FlagValue (Feature Flag Insights)</option>
              <option value="CustomEvent">Custom Events</option>
            </select>
          </div>

          {/* Feature Flag Key - only for project/environment */}
          {canUseFeatureFlagKey && (
            <div className="space-y-2">
              <Label htmlFor="featureFlagKey">Feature Flag Key (optional)</Label>
              <Input
                id="featureFlagKey"
                placeholder="e.g., my-feature-flag"
                value={featureFlagKey}
                onChange={(e) => setFeatureFlagKey(e.target.value)}
              />
              <p className="text-xs text-muted-foreground">
                Only applies to FlagValue events. Leave empty to delete all matching events.
              </p>
            </div>
          )}
        </CardContent>
      </Card>

      {/* SQL Preview */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">SQL Query</CardTitle>
          <CardDescription>The SQL query that will be executed</CardDescription>
        </CardHeader>
        <CardContent>
          <pre className="bg-muted p-4 rounded-md text-sm overflow-x-auto font-mono">
            {sqlQuery}
          </pre>
        </CardContent>
      </Card>

      {/* Preview Result */}
      {previewCount !== null && (
        <Alert>
          <AlertTriangle className="h-4 w-4" />
          <AlertTitle>Preview Result</AlertTitle>
          <AlertDescription>
            <span className="font-bold text-lg">{previewCount.toLocaleString()}</span> events will
            be deleted.
          </AlertDescription>
        </Alert>
      )}

      {/* Result Message */}
      {result && (
        <Alert variant={result.success ? 'default' : 'destructive'}>
          <AlertTitle>{result.success ? 'Success' : 'Error'}</AlertTitle>
          <AlertDescription>{result.message}</AlertDescription>
        </Alert>
      )}

      {/* Action Buttons */}
      <div className="flex gap-4">
        <Button variant="outline" onClick={handlePreview} disabled={loading}>
          <Eye className="h-4 w-4 mr-2" />
          Preview
        </Button>
        <Button
          variant="destructive"
          onClick={handleDelete}
          disabled={loading || previewCount === null || previewCount === 0}
        >
          <Trash2 className="h-4 w-4 mr-2" />
          Delete Events
        </Button>
      </div>
    </div>
  );
}
