import type { Selection } from '@/types';

interface SqlGeneratorParams {
  selection: Selection | null;
  beforeDate?: string;
  afterDate?: string;
  featureFlagKey?: string;
  eventType?: string;
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

  // Time range conditions
  if (beforeDate) {
    conditions.push(`"timestamp" < '${beforeDate}'`);
  }
  if (afterDate) {
    conditions.push(`"timestamp" >= '${afterDate}'`);
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
