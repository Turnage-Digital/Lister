import {defineConfig} from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
    plugins: [react()],
    build: {
        outDir: "../wwwroot/dist",
        emptyOutDir: true,
        lib: {
            entry: "./src/index.ts",
            name: "MyReactLibrary",
            formats: ["es"],
            fileName: (format) => `my-react-library.${format}.js`,
        },
        rollupOptions: {
            external: ["react", "react-dom"],
        },
    },
});