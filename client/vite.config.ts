import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
//import mkcert from 'vite-plugin-mkcert'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],//, mkcert()],
 preview: {
  port: 5175,
  strictPort: true,
 },
  server: { 
    https: false,
    host: true,
    port: 5175,
  },
})
