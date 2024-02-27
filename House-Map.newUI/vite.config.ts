import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server:{
    port:5173,
    headers:{
      "Access-Control-Allow-Origin":"*",
    },
    proxy:{
      '/api':{
        target:'https://house-map.cn/',
        changeOrigin:true,
       cookiePathRewrite:""
      },
      '/v2':{
        target:'https://house-map.cn/',
        changeOrigin:true,
        cookiePathRewrite:""
      }
    }
  }
})
