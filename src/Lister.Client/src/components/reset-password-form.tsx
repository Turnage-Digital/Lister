import * as React from "react";

import { Visibility, VisibilityOff } from "@mui/icons-material";
import {
  Alert,
  Button,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
} from "@mui/material";

export interface Props {
  email: string;
  code: string;
  onPasswordReset: () => Promise<void>;
}

const ResetPasswordForm = ({ email, code, onPasswordReset }: Props) => {
  const [showPassword, setShowPassword] = React.useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = React.useState(false);
  const [loading, setLoading] = React.useState(false);

  const [formErrorMessage, setFormErrorMessage] = React.useState<string | null>(
    null,
  );
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState<
    string | null
  >(null);
  const [confirmPasswordErrorMessage, setConfirmPasswordErrorMessage] =
    React.useState<string | null>(null);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (passwordErrorMessage || confirmPasswordErrorMessage) {
      return;
    }

    const data = new FormData(event.currentTarget);
    const newPassword = data.get("password") as string;
    const confirmPassword = data.get("confirmPassword") as string;

    if (newPassword !== confirmPassword) {
      setConfirmPasswordErrorMessage("Passwords do not match.");
      return;
    }

    try {
      setFormErrorMessage(null);
      setLoading(true);

      const input = {
        email,
        resetCode: code,
        newPassword,
      };
      const request = new Request("/identity/resetPassword", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(input),
      });
      const response = await fetch(request);

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        if (response.status === 400 && errorData.errors) {
          // Handle validation errors
          const errors = errorData.errors;
          if (errors.NewPassword) {
            setPasswordErrorMessage(errors.NewPassword[0]);
          } else if (errors.ResetCode) {
            setFormErrorMessage("Invalid or expired reset code.");
          } else {
            setFormErrorMessage("Invalid reset request.");
          }
        } else {
          setFormErrorMessage("Failed to reset password. Please try again.");
        }
        return;
      }

      await onPasswordReset();
    } catch {
      setFormErrorMessage("An unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  const validateInputs = () => {
    const password = document.getElementById("password") as HTMLInputElement;
    const confirmPassword = document.getElementById(
      "confirmPassword",
    ) as HTMLInputElement;

    let retval = true;

    if (password.value) {
      setPasswordErrorMessage(null);
    } else {
      setPasswordErrorMessage("Please enter a password.");
      retval = false;
    }

    if (confirmPassword.value) {
      if (password.value === confirmPassword.value) {
        setConfirmPasswordErrorMessage(null);
      } else {
        setConfirmPasswordErrorMessage("Passwords do not match.");
        retval = false;
      }
    } else {
      setConfirmPasswordErrorMessage("Please confirm your password.");
      retval = false;
    }

    return retval;
  };

  const passwordErrorColor = passwordErrorMessage ? "error" : "primary";
  const confirmPasswordErrorColor = confirmPasswordErrorMessage
    ? "error"
    : "primary";

  const showPasswordType = showPassword ? "text" : "password";
  const showPasswordIcon = showPassword ? <VisibilityOff /> : <Visibility />;
  const showConfirmPasswordType = showConfirmPassword ? "text" : "password";
  const showConfirmPasswordIcon = showConfirmPassword ? (
    <VisibilityOff />
  ) : (
    <Visibility />
  );

  return (
    <Stack
      spacing={3}
      component="form"
      method="post"
      onSubmit={handleSubmit}
      noValidate
    >
      <TextField
        margin="normal"
        id="password"
        name="password"
        placeholder="••••••"
        autoComplete="new-password"
        required
        fullWidth
        variant="outlined"
        type={showPasswordType}
        error={passwordErrorMessage !== null}
        helperText={passwordErrorMessage}
        color={passwordErrorColor}
        slotProps={{
          input: {
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  aria-label="toggle password visibility"
                  onClick={() => setShowPassword((prev) => !prev)}
                >
                  {showPasswordIcon}
                </IconButton>
              </InputAdornment>
            ),
          },
        }}
      />

      <TextField
        margin="normal"
        id="confirmPassword"
        name="confirmPassword"
        placeholder="••••••"
        autoComplete="new-password"
        required
        fullWidth
        variant="outlined"
        type={showConfirmPasswordType}
        error={confirmPasswordErrorMessage !== null}
        helperText={confirmPasswordErrorMessage}
        color={confirmPasswordErrorColor}
        slotProps={{
          input: {
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  aria-label="toggle confirm password visibility"
                  onClick={() => setShowConfirmPassword((prev) => !prev)}
                >
                  {showConfirmPasswordIcon}
                </IconButton>
              </InputAdornment>
            ),
          },
        }}
      />

      <Button
        type="submit"
        variant="contained"
        size="large"
        fullWidth
        loading={loading}
        onClick={validateInputs}
      >
        Reset Password
      </Button>

      {formErrorMessage && (
        <Alert severity="error" sx={{ width: "100%" }}>
          {formErrorMessage}
        </Alert>
      )}
    </Stack>
  );
};

export default ResetPasswordForm;