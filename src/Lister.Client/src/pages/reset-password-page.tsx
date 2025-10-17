import * as React from "react";

import { Alert, Typography } from "@mui/material";
import { useNavigate, useSearchParams } from "react-router-dom";

import { AuthPageLayout, ResetPasswordForm } from "../components";

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
    <AuthPageLayout>
      <Typography variant="h5" align="center" gutterBottom>
        Reset Password
      </Typography>

      {formContent}

      {errorMessage && <Alert severity="error">{errorMessage}</Alert>}
    </AuthPageLayout>
  );
};

export default ResetPasswordPage;
