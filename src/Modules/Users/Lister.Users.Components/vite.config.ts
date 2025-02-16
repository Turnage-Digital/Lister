import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "./wwwroot/dist/js",
    emptyOutDir: true,
    lib: {
      entry: "./src/index.ts",
      name: "react-library",
      formats: ["es"],
      fileName: "react-library",
    },
    rollupOptions: {
      external: ["react", "react-dom"],
    },
  },
});
