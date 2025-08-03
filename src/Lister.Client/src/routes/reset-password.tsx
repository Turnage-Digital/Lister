import * as React from "react";

import { Alert, Stack, Typography } from "@mui/material";
import { createFileRoute } from "@tanstack/react-router";

import { ResetPasswordForm } from "../components";

const RouteComponent = () => {
  const navigate = Route.useNavigate();
  const search = Route.useSearch();
  const [errorMessage, setErrorMessage] = React.useState<string | null>(null);

  React.useLayoutEffect(() => {
    if (!search.email || !search.code) {
      setErrorMessage(
        "Invalid reset link. Please request a new password reset.",
      );
    }
  }, [search.email, search.code]);

  const handlePasswordReset = async () => {
    await navigate({ to: "/sign-in" });
  };

  return (
    <Stack sx={{ width: "450px", mx: "auto", px: 2, py: 4 }} spacing={4}>
      <Typography variant="h5">Reset Password</Typography>

      <ResetPasswordForm
        email={search.email!}
        code={search.code!}
        onPasswordReset={handlePasswordReset}
      />

      {errorMessage && <Alert severity="error">{errorMessage}</Alert>}
    </Stack>
  );
};

export interface ResetPasswordSearch {
  email?: string;
  code?: string;
}

export const Route = createFileRoute("/reset-password")({
  component: RouteComponent,
  validateSearch: (search): ResetPasswordSearch => {
    return {
      email: search.email as string | undefined,
      code: search.code as string | undefined,
    };
  },
});
