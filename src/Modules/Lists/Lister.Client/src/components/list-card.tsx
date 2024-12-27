import * as React from "react";

import {Delete, Visibility} from "@mui/icons-material";
import {Card, CardActions, CardContent, Grid2, IconButton, Tooltip, Typography,} from "@mui/material";

import {ListName} from "../models";
import PreloadIconButton from "./preload-icon-button";

export interface Props {
    listName: ListName;
    onDeleteClick: (id: string) => void;
}

const ListCard = ({listName, onDeleteClick}: Props) => {
    return (
        <Grid2 key={listName.id} size={{xs: 12, sm: 6, md: 4}}>
            <Card>
                <CardContent>
                    <Typography gutterBottom variant="h5" component="div">
                        {listName.name}
                    </Typography>
                </CardContent>
                <CardActions sx={{justifyContent: "flex-end"}}>
                    <Tooltip title={`View ${listName.name}`}>
                        <PreloadIconButton
                            to="/$listId"
                            params={{listId: listName.id}}
                            search={{page: 0, pageSize: 10}}
                            preload="intent"
                        >
                            <Visibility/>
                        </PreloadIconButton>
                    </Tooltip>
                    <Tooltip title={`Delete ${listName.name}`}>
                        <IconButton onClick={() => onDeleteClick(listName.id)}>
                            <Delete/>
                        </IconButton>
                    </Tooltip>
                </CardActions>
            </Card>
        </Grid2>
    );
};

export default ListCard;
