import type { Selection } from '@/types';

interface SqlGeneratorParams {
  selection: Selection | null;
  beforeDate?: string;
  afterDate?: string;
  featureFlagKey?: string;
  eventType?: string;
}

/**
 * Converts a datetime-local value (e.g., "2025-10-05T11:12") 
 * to PostgreSQL timestamp format (e.g., "2025-10-05 11:12:00.000")
 */
function toPostgresTimestamp(dateTimeLocal: string): string {
  // datetime-local format: "2025-10-05T11:12" or "2025-10-05T11:12:45"
  // PostgreSQL format: "2025-10-05 11:12:45.000"
  const normalized = dateTimeLocal.replace('T', ' ');
  
  // Check if seconds are included
  const parts = normalized.split(':');
  if (parts.length === 2) {
    // Only hours and minutes, add seconds
    return `${normalized}:00.000`;
  } else if (parts.length === 3) {
    // Has seconds, check for milliseconds
    if (!normalized.includes('.')) {
      return `${normalized}.000`;
    }
    return normalized;
  }
  
  return normalized;
}

export function generateDeleteSql(params: SqlGeneratorParams): string {
  const { selection, beforeDate, afterDate, featureFlagKey, eventType } = params;

  if (!selection) {
    return '-- Please select an item from the left panel';
  }

  const conditions: string[] = [];

  // Event type filter
  if (eventType) {
    conditions.push(`"event" = '${eventType}'`);
  }

  // Time range conditions - convert to PostgreSQL timestamp format
  if (beforeDate) {
    conditions.push(`"timestamp" < '${toPostgresTimestamp(beforeDate)}'`);
  }
  if (afterDate) {
    conditions.push(`"timestamp" >= '${toPostgresTimestamp(afterDate)}'`);
  }

  // Selection-based conditions
  switch (selection.type) {
    case 'environment':
      conditions.push(`env_id = '${selection.id}'`);
      if (featureFlagKey) {
        conditions.push(`"event" = 'FlagValue'`);
        conditions.push(`properties->>'featureFlagKey' = '${featureFlagKey}'`);
      }
      break;

    case 'project':
      if (selection.envIds && selection.envIds.length > 0) {
        const envList = selection.envIds.map((id) => `'${id}'`).join(', ');
        conditions.push(`env_id IN (${envList})`);
        if (featureFlagKey) {
          conditions.push(`"event" = 'FlagValue'`);
          conditions.push(`properties->>'featureFlagKey' = '${featureFlagKey}'`);
        }
      } else {
        return '-- No environments found for this project';
      }
      break;

    case 'organization':
      if (selection.envIds && selection.envIds.length > 0) {
        const envList = selection.envIds.map((id) => `'${id}'`).join(', ');
        conditions.push(`env_id IN (${envList})`);
      } else {
        return '-- No environments found for this organization';
      }
      break;

    case 'workspace':
      if (selection.envIds && selection.envIds.length > 0) {
        const envList = selection.envIds.map((id) => `'${id}'`).join(', ');
        conditions.push(`env_id IN (${envList})`);
      } else {
        return '-- No environments found for this workspace';
      }
      break;
  }

  if (conditions.length === 0) {
    return '-- Please specify at least one filter condition';
  }

  const whereClause = conditions.join('\n  AND ');

  return `DELETE FROM events
WHERE ${whereClause};`;
}

export function generatePreviewSql(params: SqlGeneratorParams): string {
  const deleteSql = generateDeleteSql(params);
  if (deleteSql.startsWith('--')) {
    return deleteSql;
  }
  return deleteSql.replace('DELETE FROM events', 'SELECT COUNT(*) FROM events');
}
