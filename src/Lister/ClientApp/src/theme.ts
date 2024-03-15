import { createTheme } from "@mui/material";

const theme = createTheme({
  palette: {
    primary: {
      // deep purple
      main: "#A330C9",
    },
    background: {
      default: "#FAFAFA",
    },
  },
  typography: {
    fontFamily: "Inter, sans-serif",
  },
});

export default theme;
