module.exports = {
  apps: [
    {
      name: 'frontend-financeiro',
      script: './node_modules/next/dist/bin/next',
      args: 'start',
      cwd: './',
      instances: 1,
      autorestart: true,
      watch: false,
      env: {
        NODE_ENV: 'production',
        PORT: 7965,
        NEXT_PUBLIC_API_URL: 'http://localhost:1337',
      },
    },
  ],
};