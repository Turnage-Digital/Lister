import * as React from "react";

import { Link, Paper, Stack, Typography } from "@mui/material";
import { createFileRoute, useRouter } from "@tanstack/react-router";

//
import { SignUpForm } from "../components";

const RouteComponent = () => {
  const router = useRouter();
  const navigate = Route.useNavigate();
  const search = Route.useSearch();
  const { auth } = Route.useRouteContext({ select: ({ auth }) => ({ auth }) });
  const status = auth.status;

  // Removed layout-effect push; navigate after signup handles redirect

  const handleSignedUp = async (email: string) => {
    auth.login(email);
    await router.invalidate();
    await navigate({ to: search.callbackUrl || "/" });
  };

  return (
    <Stack
      sx={{ width: "450px", mx: "auto", px: 2, pt: 18, pb: 4 }}
      spacing={4}
    >
      <Paper
        elevation={1}
        sx={{
          p: 4,
          borderRadius: 2,
          transition: "box-shadow 0.2s ease-in-out",
          "&:hover": {
            elevation: 2,
          },
        }}
      >
        <Stack spacing={4}>
          <Typography variant="h5" align="center" gutterBottom>
            Sign up
          </Typography>

          <SignUpForm onSignedUp={handleSignedUp} />

          <Stack spacing={2} alignItems="center">
            <Typography
              variant="body2"
              color="text.secondary"
              align="center"
              component="div"
            >
              Already have an account?{" "}
              <Link
                component="button"
                type="button"
                onClick={() => navigate({ to: "/sign-in", search })}
                sx={{
                  textDecoration: "none",
                  transition: "color 0.2s ease-in-out",
                  display: "inline",
                  "&:hover": {
                    textDecoration: "underline",
                  },
                }}
              >
                Sign in
              </Link>
            </Typography>
          </Stack>
        </Stack>
      </Paper>
    </Stack>
  );
};

export interface SignUpSearch {
  callbackUrl?: string;
}

export const Route = createFileRoute("/sign-up")({
  component: RouteComponent,
  validateSearch: (search): SignUpSearch => {
    return {
      callbackUrl: search.callbackUrl as string | undefined,
    };
  },
});
