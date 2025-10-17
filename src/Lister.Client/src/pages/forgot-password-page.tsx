import * as React from "react";

import { Alert, Button, Stack, Typography } from "@mui/material";
import { useNavigate, useSearchParams } from "react-router-dom";

import { AuthPageLayout, ForgotPasswordForm } from "../components";

const ForgotPasswordPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [successMessage, setSuccessMessage] = React.useState<string | null>(
    null,
  );

  const handleBackToSignIn = () => {
    const query = searchParams.toString();
    navigate(query ? `/sign-in?${query}` : "/sign-in");
  };

  const handleSubmitStart = () => {
    setSuccessMessage(null);
  };

  const continueButton = successMessage ? (
    <Button variant="contained" onClick={handleBackToSignIn} fullWidth>
      Continue
    </Button>
  ) : null;

  const successAlert = successMessage ? (
    <Alert severity="success">{successMessage}</Alert>
  ) : null;

  return (
    <AuthPageLayout>
      <Stack spacing={2}>
        <Typography variant="h5" align="center" gutterBottom>
          Reset password
        </Typography>
        <Typography variant="body2" color="text.secondary" align="center">
          Enter your account&apos;s email address and we&apos;ll send you a
          reset link.
        </Typography>
      </Stack>

      <ForgotPasswordForm
        onSubmitStart={handleSubmitStart}
        onSuccess={(message) => setSuccessMessage(message)}
      />

      {successAlert}

      <Stack spacing={2} alignItems="center">
        <Button onClick={handleBackToSignIn} fullWidth>
          Back to sign in
        </Button>
        {continueButton}
      </Stack>
    </AuthPageLayout>
  );
};

export default ForgotPasswordPage;
