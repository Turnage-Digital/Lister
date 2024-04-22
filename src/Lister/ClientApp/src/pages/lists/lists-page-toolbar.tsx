import React, { MouseEvent, useEffect } from "react";
import {
  Button,
  Divider,
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
    <>
      <Grid container direction="row" alignItems="center">
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
            sx={{ ml: 1 }}
          >
            <ExpandCircleDown />
          </IconButton>
        </Grid>

        <Grid flex={1} display="flex" justifyContent="flex-end">
          <Button
            variant="contained"
            startIcon={<AddCircle />}
            onClick={() => navigate(`/${selectedListName.id}/items/create`)}
          >
            Create an Item
          </Button>
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
    </>
  ) : null;
};

export default ListsPageToolbar;
