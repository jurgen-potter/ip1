import { defineConfig } from 'vite'
import path from 'path'

export default defineConfig({
  plugins: [],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  },
  build: {
    outDir: '../wwwroot/dist/clientapp',
    assetsDir: 'assets',
    rollupOptions: {
      input: {
        main: path.resolve('src/main.ts'),
        styles: path.resolve('src/index.scss'),
        vote: path.resolve('src/ts/recommendation/vote.ts'),
        endVote: path.resolve('src/ts/recommendation/endVote.ts'),
        votersModalLoader: path.resolve('src/ts/recommendation/votersModalLoader.ts'),
        dashboard: path.resolve('src/ts/dashboard/timeline.ts'),
        postModal: path.resolve('src/ts/dashboard/createPostModal.ts'),
        recruitment: path.resolve('src/ts/recruitment/index.ts'),
        registration: path.resolve('src/ts/registration/index.ts'),
        memberRegister: path.resolve('src/ts/memberRegister/registerMember.ts'),
        admin: path.resolve('src/ts/admin/editQuestionnaire.ts'),
        account: path.resolve('src/ts/account/login.ts')
      },
      output: {
        assetFileNames: 'assets/[name][extname]',
        entryFileNames: 'assets/[name].js',
        chunkFileNames: 'assets/[name]-[hash].js'
      }
    }
  }
})