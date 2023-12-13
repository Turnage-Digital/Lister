import { createTheme } from "@mui/material";

const theme = createTheme({
  palette: {
    mode: "dark",
    primary: {
      main: "#ffc107",
    },
    secondary: {
      main: "#0745ff",
    },
  },
  typography: {
    fontFamily: "Inter, sans-serif",
  },
});

export default theme;
