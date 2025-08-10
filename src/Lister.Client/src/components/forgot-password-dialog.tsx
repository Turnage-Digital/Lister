import * as React from "react";

import { LoadingButton } from "@mui/lab";
import { Alert, Box, Button, TextField, Typography } from "@mui/material";

import {
  SideDrawerContainer,
  SideDrawerContent,
  SideDrawerFooter,
  SideDrawerHeader,
  useSideDrawer,
} from "./side-drawer";

const ForgotPasswordDialog = () => {
  const { closeDrawer } = useSideDrawer();
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

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (emailErrorMessage) {
      return;
    }

    try {
      setFormErrorMessage(null);
      setSuccessMessage(null);
      setLoading(true);

      const input = { email };
      const request = new Request("/identity/forgotPassword", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(input),
      });
      const response = await fetch(request);

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

  const validateInputs = () => {
    let retval = true;

    if (email && /\S+@\S+\.\S+/.test(email)) {
      setEmailErrorMessage(null);
    } else {
      setEmailErrorMessage("Please enter a valid email address.");
      retval = false;
    }

    return retval;
  };

  const emailErrorColor = emailErrorMessage ? "error" : "primary";

  const dialogActions = successMessage ? (
    <Button variant="contained" onClick={closeDrawer}>
      Close
    </Button>
  ) : (
    <>
      <Button onClick={closeDrawer}>Cancel</Button>
      <Box sx={{ flex: 1 }} />
      <LoadingButton
        variant="contained"
        type="submit"
        form="forgot-password-form"
        loading={loading}
        onClick={validateInputs}
      >
        Continue
      </LoadingButton>
    </>
  );

  return (
    <SideDrawerContainer>
      <SideDrawerHeader />
      <SideDrawerContent>
        <Box
          id="forgot-password-form"
          component="form"
          onSubmit={handleSubmit}
          sx={{ p: 3 }}
        >
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Enter your account&apos;s email address, and we&apos;ll send you a
            link to reset your password.
          </Typography>

          <TextField
            margin="normal"
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
            color={emailErrorColor}
          />

          {successMessage && (
            <Alert severity="success" sx={{ mt: 2 }}>
              {successMessage}
            </Alert>
          )}

          {formErrorMessage && (
            <Alert severity="error" sx={{ mt: 2 }}>
              {formErrorMessage}
            </Alert>
          )}
        </Box>
      </SideDrawerContent>
      <SideDrawerFooter>{dialogActions}</SideDrawerFooter>
    </SideDrawerContainer>
  );
};

export default ForgotPasswordDialog;
