import {createTheme} from "@mui/material";

const theme = createTheme({
    cssVariables: true,
    palette: {
        primary: {
            main: "#334E68",
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
