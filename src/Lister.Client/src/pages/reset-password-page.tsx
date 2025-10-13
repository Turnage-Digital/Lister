import * as React from "react";

import { Alert, Stack, Typography } from "@mui/material";
import { useNavigate, useSearchParams } from "react-router-dom";

import { ResetPasswordForm } from "../components";

const ResetPasswordPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [errorMessage, setErrorMessage] = React.useState<string | null>(null);

  const email = searchParams.get("email") ?? undefined;
  const resetCode = searchParams.get("code") ?? undefined;

  React.useLayoutEffect(() => {
    if (!email || !resetCode) {
      setErrorMessage(
        "Invalid reset link. Please request a new password reset.",
      );
    }
  }, [email, resetCode]);

  const handlePasswordReset = () => {
    navigate("/sign-in");
  };

  const formContent =
    email && resetCode ? (
      <ResetPasswordForm
        email={email}
        resetCode={resetCode}
        onPasswordReset={handlePasswordReset}
      />
    ) : null;

  return (
    <Stack
      sx={{
        maxWidth: 420,
        width: "100%",
        mx: "auto",
        px: { xs: 3, md: 4 },
        py: { xs: 4, md: 5 },
      }}
      spacing={{ xs: 4, md: 5 }}
    >
      <Typography variant="h5">Reset Password</Typography>

      {formContent}

      {errorMessage && <Alert severity="error">{errorMessage}</Alert>}
    </Stack>
  );
};

export default ResetPasswordPage;
