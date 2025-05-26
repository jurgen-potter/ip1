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
        loadRecommendations: path.resolve('src/ts/recommendation/loadRecommendations.ts'),
        dashboard: path.resolve('src/ts/dashboard/timeline.ts'),
        postModal: path.resolve('src/ts/dashboard/createPostModal.ts'),
        recruitment: path.resolve('src/ts/recruitment/index.ts'),
        registration: path.resolve('src/ts/registration/index.ts'),
        memberRegister: path.resolve('src/ts/memberRegister/registerMember.ts'),
        admin: path.resolve('src/ts/admin/editQuestionnaire.ts'),
        account: path.resolve('src/ts/account/login.ts'),
        manageMembers: path.resolve('src/ts/panel/manageMembers.ts'),
        manageAdmins: path.resolve('src/ts/admin/manageAdmins.ts'),
        manageOrganizations: path.resolve('src/ts/admin/manageOrganizations.ts'),
        manageStaff: path.resolve('src/ts/organization/manageStaff.ts'),
        addMember: path.resolve('src/ts/organization/addMember.ts'),
        details: path.resolve('src/ts/panel/details.ts')
      },
      output: {
        assetFileNames: 'assets/[name][extname]',
        entryFileNames: 'assets/[name].js',
        chunkFileNames: 'assets/[name]-[hash].js'
      }
    }
  }
})