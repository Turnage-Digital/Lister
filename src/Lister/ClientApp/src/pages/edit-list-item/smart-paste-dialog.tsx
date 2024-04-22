import React from "react";
import { Box, Button, TextField } from "@mui/material";
import { Save } from "@mui/icons-material";

import {
  SideDrawerContent,
  SideDrawerFooter,
  SideDrawerHeader,
  useSideDrawer,
} from "../../components";

const SmartPasteDialog = () => {
  const { closeDrawer } = useSideDrawer();

  return (
    <>
      <SideDrawerHeader />
      <SideDrawerContent>
        <TextField
          multiline
          fullWidth
          rows={10}
          placeholder="Paste your text here..."
          sx={{ p: 2 }}
        />
      </SideDrawerContent>
      <SideDrawerFooter>
        <Button onClick={closeDrawer}>Cancel</Button>
        <Box sx={{ flexGrow: 1 }} />
        <Button variant="contained" startIcon={<Save />} sx={{ ml: 2 }}>
          Submit
        </Button>
      </SideDrawerFooter>
    </>
  );
};

export default SmartPasteDialog;
