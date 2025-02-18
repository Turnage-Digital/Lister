import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "./wwwroot/dist/js",
    emptyOutDir: true,
    lib: {
      name: "UsersComponents",
      formats: ["iife"],
      entry: "./src/index.ts",
    },
    rollupOptions: {
      external: ["react", "react-dom"],
      output: {
        entryFileNames: "users-components.js",
        globals: {
          react: "React",
          "react-dom": "ReactDOM",
        },
      },
    },
  },
  define: {
    "process.env": {},
  },
});
