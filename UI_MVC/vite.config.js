import { defineConfig } from "vite";
import path from 'path';

export default defineConfig({
    base: "./",
    build: {
        outDir: 'wwwroot/dist',
        rollupOptions: {
            input: './main.js',
        }
    },
    css: {
        preprocessorOptions: {
            scss: {
                api: 'modern-compiler',
                silenceDeprecations: ['mixed-decls', 'color-functions', 'global-builtin', 'import']
            }
        }
    }
});
