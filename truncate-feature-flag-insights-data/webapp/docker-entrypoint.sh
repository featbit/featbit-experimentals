#!/bin/sh
# Extract hostname from API_BACKEND_URL for proper Host header
# Example: https://eventscleanupapi.xxx.azurecontainerapps.io -> eventscleanupapi.xxx.azurecontainerapps.io
export API_BACKEND_HOST=$(echo "$API_BACKEND_URL" | sed -e 's|^https://||' -e 's|^http://||' -e 's|/.*$||')
echo "API_BACKEND_URL: $API_BACKEND_URL"
echo "API_BACKEND_HOST: $API_BACKEND_HOST"

# Replace environment variables in nginx config template
envsubst '${API_BACKEND_URL} ${API_BACKEND_HOST}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf

# Show the generated config for debugging
echo "Generated nginx config:"
cat /etc/nginx/conf.d/default.conf

# Start nginx
exec nginx -g 'daemon off;'
