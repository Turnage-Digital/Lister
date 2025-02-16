import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "./wwwroot/dist/js",
    emptyOutDir: true,
    lib: {
      entry: "./src/index.ts",
      formats: ["es"],
      fileName: "users-components",
    },
  },
  define: {
    "process.env": {},
  },
});
