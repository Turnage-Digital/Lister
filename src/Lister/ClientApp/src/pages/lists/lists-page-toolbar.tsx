import React, { MouseEvent, useEffect } from "react";
import {
  Box,
  Button,
  Divider,
  Hidden,
  IconButton,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { AddCircle, ExpandCircleDown, PlaylistAdd } from "@mui/icons-material";
import { useNavigate, useParams } from "react-router-dom";

import { ListName } from "../../models";

interface Props {
  listNames: ListName[];
  onSelectedListNameChanged: (list: ListName) => void;
}

const ListsPageToolbar = ({ listNames, onSelectedListNameChanged }: Props) => {
  const params = useParams();
  const navigate = useNavigate();

  const selectedListName = listNames.find((list) => list.id === params.listId);

  useEffect(() => {
    if (!selectedListName && listNames.length > 0) {
      navigate(`/${listNames[0].id}`);
    }
  }, [listNames, navigate, selectedListName]);

  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleButtonClick = (event: MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuItemClick = (list: ListName) => {
    setAnchorEl(null);
    onSelectedListNameChanged(list);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return selectedListName ? (
    <Box alignItems="center" sx={{ display: "flex" }}>
      <Box sx={{ flexGrow: 1 }}>
        <Grid container direction="row" alignItems="center" spacing={2}>
          <Grid>
            <Typography
              color="primary"
              fontWeight="medium"
              variant="h4"
              component="h1"
            >
              {selectedListName.name}
            </Typography>
          </Grid>
          <Grid>
            <IconButton
              color="primary"
              size="large"
              onClick={handleButtonClick}
              sx={{ ml: 2 }}
            >
              <ExpandCircleDown />
            </IconButton>
          </Grid>
        </Grid>

        <Menu anchorEl={anchorEl} open={open} onClose={handleMenuClose}>
          {listNames.map((list) => (
            <MenuItem
              key={list.id}
              selected={list.id === selectedListName.id}
              onClick={() => handleMenuItemClick(list)}
            >
              {list.name}
            </MenuItem>
          ))}
          <Divider />
          <MenuItem onClick={() => navigate("/create")}>
            <ListItemIcon>
              <PlaylistAdd fontSize="small" />
            </ListItemIcon>
            <ListItemText>Create a List</ListItemText>
          </MenuItem>
        </Menu>
      </Box>

      <Hidden mdDown>
        <Box
          sx={{
            flexGrow: 2,
            display: "flex",
            justifyContent: "flex-end",
          }}
        >
          <Button
            variant="contained"
            startIcon={<AddCircle />}
            onClick={() => navigate(`/${selectedListName.id}/items/create`)}
          >
            Create an Item
          </Button>
        </Box>
      </Hidden>
    </Box>
  ) : null;
};

export default ListsPageToolbar;
