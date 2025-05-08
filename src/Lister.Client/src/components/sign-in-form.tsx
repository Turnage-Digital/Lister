import * as React from "react";

import { Visibility, VisibilityOff } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import {
  Alert,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
} from "@mui/material";

export interface Props {
  onSignedIn: (email: string) => Promise<void>;
}

const SignInForm = ({ onSignedIn }: Props) => {
  const [showPassword, setShowPassword] = React.useState(false);
  const [loading, setLoading] = React.useState(false);

  const [formErrorMessage, setFormErrorMessage] = React.useState<string | null>(
    null,
  );
  const [emailErrorMessage, setEmailErrorMessage] = React.useState<
    string | null
  >(null);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState<
    string | null
  >(null);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (emailErrorMessage || passwordErrorMessage) {
      return;
    }

    const data = new FormData(event.currentTarget);
    const email = data.get("email") as string;
    const password = data.get("password") as string;

    try {
      setFormErrorMessage(null);
      setLoading(true);

      const input = { email, password };
      const request = new Request("/identity/login?useCookies=true", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(input),
      });
      const response = await fetch(request);
      if (!response.ok) {
        setFormErrorMessage("Invalid email or password.");
        return;
      }

      await onSignedIn(email);
    } catch {
      setFormErrorMessage("An unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  const validateInputs = () => {
    const email = document.getElementById("email") as HTMLInputElement;
    const password = document.getElementById("password") as HTMLInputElement;

    let retval = true;

    if (email.value && /\S+@\S+\.\S+/.test(email.value)) {
      setEmailErrorMessage(null);
    } else {
      setEmailErrorMessage("Please enter a valid email address.");
      retval = false;
    }

    if (password.value) {
      setPasswordErrorMessage(null);
    } else {
      setPasswordErrorMessage("Please enter a password.");
      retval = false;
    }

    return retval;
  };

  const emailErrorColor = emailErrorMessage ? "error" : "primary";
  const passwordErrorColor = passwordErrorMessage ? "error" : "primary";

  const showPasswordType = showPassword ? "text" : "password";
  const showPasswordIcon = showPassword ? <VisibilityOff /> : <Visibility />;

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
        id="email"
        name="email"
        placeholder="your@email.com"
        autoComplete="email"
        required
        fullWidth
        variant="outlined"
        type="email"
        error={emailErrorMessage !== null}
        helperText={emailErrorMessage}
        color={emailErrorColor}
      />

      <TextField
        margin="normal"
        id="password"
        name="password"
        placeholder="••••••"
        autoComplete="current-password"
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

      <LoadingButton
        type="submit"
        variant="contained"
        size="large"
        fullWidth
        loading={loading}
        onClick={validateInputs}
      >
        Sign in
      </LoadingButton>

      {formErrorMessage && (
        <Alert severity="error" sx={{ width: "100%" }}>
          {formErrorMessage}
        </Alert>
      )}
    </Stack>
  );
};

export default SignInForm;
