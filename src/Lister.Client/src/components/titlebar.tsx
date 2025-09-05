import * as React from "react";

import { ChevronRight } from "@mui/icons-material";
import {
  Box,
  Breadcrumbs,
  Button,
  Grid,
  Link,
  Stack,
  Typography,
  useTheme,
} from "@mui/material";

export interface Action {
  title: string;
  icon?: React.ReactNode;
  onClick?: () => void;
}

export interface Breadcrumb {
  title: string;
  onClick?: () => void;
}

export interface TitlebarProps {
  title: string;
  actions?: Action[];
  breadcrumbs?: Breadcrumb[];
}

const Titlebar = ({ title, actions, breadcrumbs }: TitlebarProps) => {
  const theme = useTheme();

  return (
    <Box sx={{ mb: 4 }}>
      {/* Title and Actions Section */}
      <Grid container alignItems="center" spacing={2} sx={{ mb: 2 }}>
        <Grid size={{ xs: 12, md: actions && actions.length > 0 ? 8 : 12 }}>
          <Typography
            variant="h4"
            component="h1"
            sx={{
              fontWeight: 700,
              fontSize: { xs: "1.75rem", md: "2.125rem" },
              lineHeight: 1.2,
            }}
          >
            {title}
          </Typography>
        </Grid>

        {actions && actions.length > 0 && (
          <Grid
            size={{ xs: 12, md: 4 }}
            sx={{
              display: "flex",
              justifyContent: { xs: "flex-start", md: "flex-end" },
            }}
          >
            <Stack direction="row" spacing={1.5}>
              {actions.map((action, index) => (
                <Button
                  key={action.title}
                  variant={index === 0 ? "contained" : "outlined"}
                  startIcon={action.icon}
                  onClick={action.onClick}
                  sx={{
                    boxShadow: index === 0 ? theme.shadows[2] : "none",
                    "&:hover": {
                      boxShadow:
                        index === 0 ? theme.shadows[4] : theme.shadows[1],
                    },
                  }}
                >
                  {action.title}
                </Button>
              ))}
            </Stack>
          </Grid>
        )}
      </Grid>

      {/* Breadcrumbs Section */}
      {breadcrumbs && breadcrumbs.length > 0 && (
        <Box>
          <Breadcrumbs
            separator={
              <ChevronRight sx={{ fontSize: 16, color: "text.secondary" }} />
            }
          >
            {breadcrumbs.map((breadcrumb) => (
              <Link
                key={breadcrumb.title}
                underline="none"
                onClick={breadcrumb.onClick}
                sx={{
                  cursor: "pointer",
                  fontSize: "0.875rem",
                  fontWeight: 500,
                  color: "text.secondary",
                  transition: "color 0.2s ease-in-out",
                  "&:hover": {
                    color: "primary.main",
                  },
                }}
              >
                {breadcrumb.title}
              </Link>
            ))}
            <Typography
              sx={{
                fontSize: "0.875rem",
                fontWeight: 500,
                color: "primary.main",
              }}
            >
              {title}
            </Typography>
          </Breadcrumbs>
        </Box>
      )}
    </Box>
  );
};

export default Titlebar;
