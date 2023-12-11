import React from "react";
import { Stack, TextField } from "@mui/material";

interface Props {
  name: string | null;
  onNameChanged: (name: string) => void;
}

const NameBlock = ({ name, onNameChanged }: Props) => {
  return (
    <Stack spacing={2}>
      <TextField
        name="name"
        id="name"
        label="Name"
        margin="normal"
        required
        fullWidth
        value={name ?? ""}
        onChange={(event) => {
          onNameChanged(event.target.value);
        }}
        // sx={{
        //   background: "white",
        // }}
      />
    </Stack>
  );
};

export default NameBlock;
