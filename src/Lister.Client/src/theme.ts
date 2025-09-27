import { alpha, createTheme } from "@mui/material";

const colors = {
  // Primary Brand Colors
  primary: {
    50: "#f0f4f8",
    100: "#d9e6f2",
    200: "#b3cde3",
    300: "#8db4d3",
    400: "#679bc4",
    500: "#334E68",
    600: "#2c4357",
    700: "#253846",
    800: "#1e2d35",
    900: "#172124",
  },
  // Secondary/Accent Colors
  secondary: {
    50: "#fdf2f8",
    100: "#fce7f3",
    200: "#fbcfe8",
    300: "#f9a8d4",
    400: "#f472b6",
    500: "#ec4899",
    600: "#db2777",
    700: "#be185d",
    800: "#9d174d",
    900: "#831843",
  },
  // Semantic Colors
  success: {
    50: "#f0fdf4",
    100: "#dcfce7",
    200: "#bbf7d0",
    300: "#86efac",
    400: "#4ade80",
    500: "#22c55e",
    600: "#16a34a",
    700: "#15803d",
    800: "#166534",
    900: "#14532d",
  },
  warning: {
    50: "#fffbeb",
    100: "#fef3c7",
    200: "#fde68a",
    300: "#fcd34d",
    400: "#fbbf24",
    500: "#f59e0b",
    600: "#d97706",
    700: "#b45309",
    800: "#92400e",
    900: "#78350f",
  },
  error: {
    50: "#fef2f2",
    100: "#fee2e2",
    200: "#fecaca",
    300: "#fca5a5",
    400: "#f87171",
    500: "#ef4444",
    600: "#dc2626",
    700: "#b91c1c",
    800: "#991b1b",
    900: "#7f1d1d",
  },
  // Neutral Grays
  gray: {
    50: "#fafafa",
    100: "#f5f5f5",
    200: "#e5e5e5",
    300: "#d4d4d4",
    400: "#a3a3a3",
    500: "#737373",
    600: "#525252",
    700: "#404040",
    800: "#262626",
    900: "#171717",
  },
};

const theme = createTheme({
  cssVariables: true,

  palette: {
    mode: "light",
    primary: {
      light: colors.primary[400],
      main: colors.primary[500],
      dark: colors.primary[700],
      contrastText: "#ffffff",
    },
    secondary: {
      light: colors.secondary[400],
      main: colors.secondary[500],
      dark: colors.secondary[700],
      contrastText: "#ffffff",
    },
    error: {
      light: colors.error[400],
      main: colors.error[500],
      dark: colors.error[700],
      contrastText: "#ffffff",
    },
    warning: {
      light: colors.warning[400],
      main: colors.warning[500],
      dark: colors.warning[700],
      contrastText: colors.gray[900],
    },
    success: {
      light: colors.success[400],
      main: colors.success[500],
      dark: colors.success[700],
      contrastText: "#ffffff",
    },
    background: {
      default: colors.gray[50],
      paper: "#ffffff",
    },
    text: {
      primary: colors.gray[900],
      secondary: colors.gray[600],
      disabled: colors.gray[400],
    },
    divider: colors.gray[200],
    action: {
      active: colors.gray[600],
      hover: alpha(colors.gray[500], 0.04),
      selected: alpha(colors.primary[500], 0.08),
      disabled: colors.gray[300],
      disabledBackground: colors.gray[100],
    },
  },

  typography: {
    fontFamily:
      '"Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
    fontWeightLight: 300,
    fontWeightRegular: 400,
    fontWeightMedium: 500,
    fontWeightBold: 600,

    h1: {
      fontSize: "2.5rem",
      fontWeight: 600,
      lineHeight: 1.2,
      letterSpacing: "-0.025em",
    },
    h2: {
      fontSize: "2rem",
      fontWeight: 600,
      lineHeight: 1.25,
      letterSpacing: "-0.02em",
    },
    h3: {
      fontSize: "1.75rem",
      fontWeight: 600,
      lineHeight: 1.3,
      letterSpacing: "-0.015em",
    },
    h4: {
      fontSize: "1.5rem",
      fontWeight: 600,
      lineHeight: 1.35,
      letterSpacing: "-0.01em",
    },
    h5: {
      fontSize: "1.25rem",
      fontWeight: 600,
      lineHeight: 1.4,
      letterSpacing: "-0.005em",
    },
    h6: {
      fontSize: "1.125rem",
      fontWeight: 600,
      lineHeight: 1.45,
    },
    subtitle1: {
      fontSize: "1rem",
      fontWeight: 500,
      lineHeight: 1.5,
    },
    subtitle2: {
      fontSize: "0.875rem",
      fontWeight: 500,
      lineHeight: 1.5,
    },
    body1: {
      fontSize: "1rem",
      fontWeight: 400,
      lineHeight: 1.5,
    },
    body2: {
      fontSize: "0.875rem",
      fontWeight: 400,
      lineHeight: 1.5,
    },
    caption: {
      fontSize: "0.75rem",
      fontWeight: 400,
      lineHeight: 1.4,
      letterSpacing: "0.025em",
    },
    overline: {
      fontSize: "0.75rem",
      fontWeight: 500,
      lineHeight: 1.4,
      letterSpacing: "0.1em",
      textTransform: "uppercase",
    },
  },

  spacing: (factor: number) => `${0.25 * factor}rem`,

  shadows: [
    "none",
    "0 1px 2px 0 rgb(0 0 0 / 0.05)",
    "0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)",
    "0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)",
    "0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)",
    "0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
    "0 25px 50px -12px rgb(0 0 0 / 0.25)",
  ],

  shape: {
    borderRadius: 4,
  },

  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 6,
          border: `1px solid ${colors.gray[200]}`,
          transition: "all 0.2s ease-in-out",
          "&:hover": {
            transform: "translateY(-2px)",
            boxShadow:
              "0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)",
          },
        },
      },
      variants: [
        {
          props: { variant: "outlined" },
          style: {
            backgroundColor: "#ffffff",
            border: `2px solid ${colors.gray[200]}`,
            boxShadow: "none",
            "&:hover": {
              borderColor: colors.primary[300],
              transform: "none",
              boxShadow:
                "0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)",
            },
          },
        },
      ],
    },

    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 4,
          textTransform: "none",
          fontWeight: 500,
          fontSize: "0.875rem",
          padding: "0.625rem 1rem",
          transition: "all 0.2s ease-in-out",
          "&:hover": {
            transform: "translateY(-1px)",
          },
        },
        containedPrimary: {
          background: `linear-gradient(135deg, ${colors.primary[500]} 0%, ${colors.primary[600]} 100%)`,
          boxShadow: `0 4px 12px ${alpha(colors.primary[500], 0.3)}`,
          "&:hover": {
            background: `linear-gradient(135deg, ${colors.primary[600]} 0%, ${colors.primary[700]} 100%)`,
            boxShadow: `0 8px 20px ${alpha(colors.primary[500], 0.4)}`,
          },
        },
        outlined: {
          borderWidth: "1.5px",
          "&:hover": {
            borderWidth: "1.5px",
            backgroundColor: alpha(colors.primary[50], 0.5),
          },
        },
      },
    },

    MuiPaper: {
      styleOverrides: {
        root: {
          borderRadius: 4,
          border: `1px solid ${colors.gray[200]}`,
        },
      },
    },

    MuiTextField: {
      styleOverrides: {
        root: {
          "& .MuiOutlinedInput-root": {
            borderRadius: 4,
            transition: "all 0.2s ease-in-out",
            "& fieldset": {
              borderColor: colors.gray[300],
              borderWidth: "1.5px",
            },
            "&:hover fieldset": {
              borderColor: colors.primary[400],
            },
            "&.Mui-focused fieldset": {
              borderColor: colors.primary[500],
              borderWidth: "2px",
              boxShadow: `0 0 0 3px ${alpha(colors.primary[500], 0.1)}`,
            },
          },
          "& .MuiInputLabel-outlined": {
            fontWeight: 500,
            color: colors.gray[600],
          },
        },
      },
    },

    MuiChip: {
      styleOverrides: {
        root: {
          borderRadius: 6,
          fontWeight: 500,
          fontSize: "0.75rem",
        },
        filled: {
          "&.MuiChip-colorPrimary": {
            backgroundColor: colors.primary[100],
            color: colors.primary[800],
          },
          "&.MuiChip-colorSecondary": {
            backgroundColor: colors.secondary[100],
            color: colors.secondary[800],
          },
        },
      },
    },

    MuiDivider: {
      styleOverrides: {
        root: {
          borderColor: colors.gray[200],
        },
      },
    },

    MuiDataGrid: {
      styleOverrides: {
        root: {
          borderRadius: 4,
          border: `1px solid ${colors.gray[200]}`,
          "& .MuiDataGrid-columnHeaders": {
            backgroundColor: colors.gray[50],
            borderBottom: `1px solid ${colors.gray[200]}`,
          },
          "& .MuiDataGrid-cell": {
            borderBottom: `1px solid ${colors.gray[200]}`,
          },
          "& .MuiDataGrid-row:hover": {
            backgroundColor: colors.gray[50],
          },
        },
      },
    },
  },
});

export default theme;
