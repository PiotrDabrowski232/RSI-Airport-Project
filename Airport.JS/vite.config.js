// filepath: c:\Users\socik\Documents\AirportSOAP\RSI-Airport-Project\Airport.JS\vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'

export default defineConfig({
  plugins: [
    react(),
    basicSsl()
  ],
  server: {
    proxy: {
      '/api': {
        target: 'https://192.168.100.1:7123',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
})