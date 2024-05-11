import { ContentPaste } from "@mui/icons-material";
import { Box, Button, TextField } from "@mui/material";
import React, { useState } from "react";

import {
  SideDrawerContainer,
  SideDrawerContent,
  SideDrawerFooter,
  SideDrawerHeader,
  useSideDrawer,
} from "../../components";

interface Props {
  onPaste: (text: string) => void;
}

const SmartPasteDialog = ({ onPaste }: Props) => {
  const { closeDrawer } = useSideDrawer();
  const [text, setText] = useState("");

  return (
    <SideDrawerContainer>
      <SideDrawerHeader />
      <SideDrawerContent>
        <TextField
          multiline
          fullWidth
          rows={10}
          placeholder="Paste your text here..."
          value={text}
          onChange={(e) => setText(e.target.value)}
          sx={{ p: 2, flex: 1 }}
        />
      </SideDrawerContent>
      <SideDrawerFooter>
        <Button onClick={closeDrawer}>Cancel</Button>
        <Box sx={{ flex: 1 }} />
        <Button
          variant="contained"
          startIcon={<ContentPaste />}
          sx={{ ml: 2 }}
          onClick={() => onPaste(text)}
        >
          Smart Paste
        </Button>
      </SideDrawerFooter>
    </SideDrawerContainer>
  );
};

export default SmartPasteDialog;
