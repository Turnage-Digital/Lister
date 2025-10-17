import * as React from "react";

import { LoadingButton } from "@mui/lab";
import {
  Alert,
  Box,
  Button,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useNavigate, useSearchParams } from "react-router-dom";

const ForgotPasswordPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [loading, setLoading] = React.useState(false);
  const [email, setEmail] = React.useState("");
  const [formErrorMessage, setFormErrorMessage] = React.useState<string | null>(
    null,
  );
  const [emailErrorMessage, setEmailErrorMessage] = React.useState<
    string | null
  >(null);
  const [successMessage, setSuccessMessage] = React.useState<string | null>(
    null,
  );

  const validateInputs = () => {
    if (email && /\S+@\S+\.\S+/.test(email)) {
      setEmailErrorMessage(null);
      return true;
    }

    setEmailErrorMessage("Please enter a valid email address.");
    return false;
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!validateInputs()) {
      return;
    }

    try {
      setFormErrorMessage(null);
      setSuccessMessage(null);
      setLoading(true);

      const response = await fetch("/identity/forgotPassword", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });

      if (!response.ok) {
        if (response.status === 400) {
          const errorData = await response.json();
          if (errorData.errors?.Email) {
            setEmailErrorMessage(errorData.errors.Email[0]);
          } else {
            setEmailErrorMessage("Please enter a valid email address.");
          }
        } else {
          setFormErrorMessage("Failed to send reset email. Please try again.");
        }
        return;
      }

      setSuccessMessage("Password reset link sent! Check your email.");
    } catch {
      setFormErrorMessage("An unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  const handleBackToSignIn = () => {
    const query = searchParams.toString();
    navigate(query ? `/sign-in?${query}` : "/sign-in");
  };

  const primaryAction = successMessage ? (
    <Button variant="contained" onClick={handleBackToSignIn} fullWidth>
      Continue
    </Button>
  ) : (
    <LoadingButton
      type="submit"
      variant="contained"
      loading={loading}
      fullWidth
    >
      Send reset link
    </LoadingButton>
  );

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
          "&:hover": { elevation: 2 },
        }}
      >
        <Stack component="form" spacing={4} onSubmit={handleSubmit}>
          <Box>
            <Typography variant="h5" align="center" gutterBottom>
              Reset password
            </Typography>
            <Typography variant="body2" color="text.secondary" align="center">
              Enter your account&apos;s email address and we&apos;ll send you a
              reset link.
            </Typography>
          </Box>

          <TextField
            id="email"
            name="email"
            placeholder="your@email.com"
            autoComplete="email"
            required
            fullWidth
            variant="outlined"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            error={emailErrorMessage !== null}
            helperText={emailErrorMessage}
            inputRef={(node) => {
              if (node) {
                node.focus();
              }
            }}
          />

          {successMessage && <Alert severity="success">{successMessage}</Alert>}

          {formErrorMessage && (
            <Alert severity="error">{formErrorMessage}</Alert>
          )}

          <Stack spacing={2} alignItems="center">
            <Button onClick={handleBackToSignIn} fullWidth>
              Back to sign in
            </Button>
            {primaryAction}
          </Stack>
        </Stack>
      </Paper>
    </Stack>
  );
};

export default ForgotPasswordPage;
