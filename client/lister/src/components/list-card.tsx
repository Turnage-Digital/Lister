import { Delete, Visibility } from "@mui/icons-material";
import {
  Card,
  CardActions,
  CardContent,
  IconButton,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import React from "react";

import { ListName } from "../api";

export interface Props {
  listName: ListName;
  onViewClick: (id: string) => void;
}

const ListCard = ({ listName, onViewClick }: Props) => {
  return (
    <Grid key={listName.id} xs={12} sm={6} md={4}>
      <Card>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {listName.name}
          </Typography>
        </CardContent>
        <CardActions sx={{ justifyContent: "flex-end" }}>
          <IconButton onClick={() => onViewClick(listName.id)}>
            <Visibility />
          </IconButton>
          <IconButton>
            <Delete />
          </IconButton>
        </CardActions>
      </Card>
    </Grid>
  );
};

export default ListCard;
