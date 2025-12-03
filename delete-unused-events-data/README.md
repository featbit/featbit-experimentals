# Delete Unused Events Data

FeatBit can records feature flag insights data and events data for data analysis and debugging purposes. However, over time, unused events data may accumulate in the database, leading to increased storage usage and potential performance degradation. To address this issue, this project demostrates how to delete unused events data from the database.

## Feature Flag Insights Data

Feature flag insights data refers to the data collected about the usage and performance of feature flags within an application. Each time a feature flag is evaluated for a specified user (context instance), an event is generated and recorded in the database. This data can include information such as the feature flag key, the user context, the timestamp of the evaluation, and the result of the evaluation (e.g., whether the feature was enabled or disabled for that user).

## Custom Events Data

Custom events data refers to user-defined events that are tracked within an application. These events can represent various user actions or occurrences, such as button clicks, page views, or specific interactions with features. Each custom event is recorded in the database with details such as the event name, user context, timestamp, and any additional metadata associated with the event.

# Task 

The task involves identifying and removing events data that is no longer needed or relevant. This may include events that are older than a certain date, events associated with deleted feature flags, or events that have not been accessed for a specified period.


# Solution

## PostgreSQL

The data are stored in the table named `events` in PostgreSQL database. Below is the definition of the `events` table:

```sql
CREATE TABLE events (
	id uuid NOT NULL,
	distinct_id varchar NULL,
	env_id varchar NULL,
	"event" varchar NULL,
	properties jsonb NULL,
	"timestamp" timestamp NOT NULL
);
```

### Data Examples

Feature Flag Insights Data Example:

| id | distinct_id | env_id | event | properties | timestamp |
|----|-------------|--------|-------|------------|-----------|
| c2186a72-d009-434a-bb1b-aa41f5fa2384 | ff23468c-dec2-4bca-af50-a90c32c15933 | FlagValue |{"envId": "e73fff7b-af28-46cb-b770-a006cb770e6d", "route": "/Variation/GetMultiOptionVariation", "tag_0": "featgen-demo-user-key", "tag_1": "e74f3394-9364-48d0-b652-e62e06a768f7", "tag_2": "true", "tag_3": "featgen-user", "flagId": "e73fff7b-af28-46cb-b770-a006cb770e6d-test", "userName": "featgen-user", "accountId": "", "projectId": "", "userKeyId": "featgen-demo-user-key", "variationId": "e74f3394-9364-48d0-b652-e62e06a768f7", "featureFlagKey": "test", "sendToExperiment": true} | 2025-10-02 15:51:24.326 |

Custom Events Data Example:

### Queries to Delete Unused Events Data

Delete all flag insights data before a specific date:
```sql
DELETE FROM events
WHERE "event" = 'FlagValue' AND "timestamp" < '2025-10-04 13:56:24.138';
```

## MongoDB

## ClickHouse 

# Project Details

- Use Asp.NET Core for backend development.
- Use Entity Framework Core for database operations.
