import * as React from "react";

import { Visibility, VisibilityOff } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import { Alert, Checkbox, FormControlLabel, IconButton, InputAdornment, Stack, TextField } from "@mui/material";

export interface Props {
  onSignedIn: (email: string) => void;
}

const SignInForm = ({ onSignedIn }: Props) => {
  const [showPassword, setShowPassword] = React.useState(false);
  const [loading, setLoading] = React.useState(false);

  const [formErrorMessage, setFormErrorMessage] = React.useState<string | null>(
    null
  );
  const [emailErrorMessage, setEmailErrorMessage] = React.useState<
    string | null
  >(null);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState<
    string | null
  >(null);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    if (emailErrorMessage || passwordErrorMessage) {
      event.preventDefault();
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
          "Content-Type": "application/json"
        },
        method: "POST",
        body: JSON.stringify(input)
      });
      const response = await fetch(request);
      if (!response.ok) {
        setFormErrorMessage("Invalid username or password");
        return;
      }

      onSignedIn(email);
    } catch (e: any) {
      setFormErrorMessage(e.message);
    } finally {
      setLoading(false);
    }
  };

  const validateInputs = () => {
    const email = document.getElementById("email") as HTMLInputElement;
    const password = document.getElementById("password") as HTMLInputElement;

    let retval = true;

    if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
      setEmailErrorMessage("Please enter a valid email address.");
      retval = false;
    } else {
      setEmailErrorMessage(null);
    }

    if (!password.value || password.value.length < 6) {
      setPasswordErrorMessage("Password must be at least 6 characters long.");
      retval = false;
    } else {
      setPasswordErrorMessage(null);
    }

    return retval;
  };

  const emailErrorColor = emailErrorMessage ? "error" : "primary";
  const passwordErrorColor = passwordErrorMessage ? "error" : "primary";

  const showPasswordType = showPassword ? "text" : "password";
  const showPasswordIcon = showPassword ? <VisibilityOff /> : <Visibility />;

  return (
    <Stack
      spacing={2}
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
            )
          }
        }}
      />

      <FormControlLabel
        control={<Checkbox value="remember" color="primary" />}
        label="Remember me"
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
