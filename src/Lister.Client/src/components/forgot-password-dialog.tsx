import * as React from "react";

import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
} from "@mui/material";

interface Props {
  open: boolean;
  onClose: () => void;
}

const ForgotPasswordDialog = ({ open, onClose }: Props) => {
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
    <Button variant="contained" onClick={onClose}>
      Close
    </Button>
  ) : (
    <>
      <Button onClick={onClose}>Cancel</Button>
      <Button
        variant="contained"
        type="submit"
        loading={loading}
        onClick={validateInputs}
      >
        Continue
      </Button>
    </>
  );

  return (
    <Dialog
      open={open}
      onClose={onClose}
      slotProps={{
        paper: {
          component: "form",
          onSubmit: handleSubmit,
        },
      }}
    >
      <DialogTitle>Reset password</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Enter your account&apos;s email address, and we&apos;ll send you a
          link to reset your password.
        </DialogContentText>

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
      </DialogContent>

      <DialogActions sx={{ p: 3 }}>{dialogActions}</DialogActions>
    </Dialog>
  );
};

export default ForgotPasswordDialog;
