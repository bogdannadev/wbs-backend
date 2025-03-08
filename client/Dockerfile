# Build stage with Node 18 (compatible with React Scripts)
FROM node:18-alpine as build
WORKDIR /app

# Copy package.json for dependency resolution
COPY package.json ./

# Install TypeScript 4.9.5 first to satisfy React Scripts
RUN npm install --no-save typescript@4.9.5

# Install dependencies with legacy-peer-deps to avoid version conflicts
RUN npm install --legacy-peer-deps

# Now install i18n packages with legacy-peer-deps to handle version conflicts
RUN npm install --legacy-peer-deps --save \
    i18next@23.10.0 \
    i18next-browser-languagedetector@7.2.0 \
    react-i18next@14.0.5

# Copy the rest of the application
COPY . .

# Verify installed packages
RUN echo "Checking installed packages..." && \
    npm list typescript i18next react-i18next i18next-browser-languagedetector || true

# Build the app with legacy-peer-deps flag
RUN npm run build --legacy-peer-deps

# Production stage
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
