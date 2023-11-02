import { createTheme } from "@mui/material";

const rootElement = document.getElementById("root");
const theme = createTheme({
  palette: {
    primary: {
      main: "#0c95bf",
    },
    secondary: {
      main: "#0c95bf",
    },
    background: {
      default: "#F5F5F5",
    },
  },
  typography: {
    fontFamily: "Inter, sans-serif",
  },
});

export default theme;
