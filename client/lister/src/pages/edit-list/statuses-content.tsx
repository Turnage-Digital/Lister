import React, { useState } from "react";
import {
  Box,
  Button,
  Chip,
  FormControl,
  IconButton,
  InputAdornment,
  InputLabel,
  ListItemText,
  Menu,
  MenuItem,
  OutlinedInput,
  Stack,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";

import { Status, statusColors } from "../../api";
import { StatusBullet } from "../../components";

interface Props {
  statuses: Status[] | null;
  onStatusesChanged: (statuses: Status[]) => void;
}

const StatusesContent = ({ statuses, onStatusesChanged }: Props) => {
  const [statusName, setStatusName] = useState<string | null>(null);
  const [statusColor, setStatusColor] = useState(statusColors[0]);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);
  const menuOpen = Boolean(anchorEl);

  const handleAddClicked = () => {
    const notNull = statuses ?? [];
    const added = [...notNull, { name: statusName!, color: statusColor.value }];

    setStatusName(null);
    setStatusColor(statusColors[0]);
    setSelectedIndex(0);
    onStatusesChanged(added);
  };

  const handleRemoveClicked = (name: string) => {
    const notNull = statuses ?? [];
    const updated = notNull.filter((sd) => sd.name !== name);

    onStatusesChanged(updated);
  };

  const handleMenuItemClicked = (index: number) => {
    setStatusColor(statusColors[index]);
    setSelectedIndex(index);
    setAnchorEl(null);
  };

  const handleMenuOpenClicked = (element: HTMLButtonElement): void => {
    setAnchorEl(element);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Stack spacing={2}>
        <Stack direction="row" spacing={2}>
          <FormControl variant="outlined" margin="normal" fullWidth>
            <InputLabel htmlFor="statusName">Status</InputLabel>
            <OutlinedInput
              id="statusName"
              name="statusName"
              type="text"
              label="Status"
              sx={{
                background: "white",
              }}
              value={statusName ?? ""}
              onChange={(event) => setStatusName(event.target.value)}
              endAdornment={
                <InputAdornment position="end">
                  <IconButton
                    onClick={(event) =>
                      handleMenuOpenClicked(event.currentTarget)
                    }
                  >
                    <StatusBullet statusColor={statusColor} />
                  </IconButton>
                </InputAdornment>
              }
            />
          </FormControl>

          <Box sx={{ display: "flex", alignItems: "center" }}>
            <Button
              variant="contained"
              color="primary"
              onClick={handleAddClicked}
              disabled={!statusName}
            >
              Add
            </Button>
          </Box>
        </Stack>

        <Grid container spacing={2}>
          {statuses &&
            statuses.map((status) => (
              <Grid key={status.name}>
                <Chip
                  label={status.name}
                  sx={{
                    color: "white",
                    backgroundColor: status.color,
                  }}
                  onDelete={() => handleRemoveClicked(status.name)}
                />
              </Grid>
            ))}
        </Grid>
      </Stack>

      <Menu anchorEl={anchorEl} open={menuOpen} onClose={handleMenuClose}>
        {statusColors.map((sc, index) => (
          <MenuItem
            key={sc.name}
            selected={index === selectedIndex}
            onClick={() => handleMenuItemClicked(index)}
          >
            <StatusBullet statusColor={sc} />
            <ListItemText primary={sc.name} sx={{ px: 2 }} />
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default StatusesContent;
