import { createTheme } from "@mui/material";

const theme = createTheme({
  palette: {
    primary: {
      // Dark Slate Blue
      main: "#483D8B",
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
