import styled from "styled-components";
import {
  Card,
  CardContent,
  Container,
  IconButton,
  ListItem,
  ListItemText,
  Stack,
} from "@mui/material";
import List from "@mui/material/List";

import Screen from "types/screen";
import { FC } from "react";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import SimpleBar from "simplebar-react";
import "simplebar/dist/simplebar.min.css";
import "./index.css";
import Typography from "@mui/material/Typography";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;

interface ScreensListProps {
  screens?: Screen[];
  onDelete: Function;
  onEdit: Function;
}

export const ScreensList: FC<ScreensListProps> = (props) => {
  return (
    <div>
      <Wrapper>
        <Card
          sx={{
            width: "100%",
            maxWidth: "100%",
            boxShadow: "6",
          }}
        >
          <SimpleBar
            autoHide={false}
            style={{
              maxHeight: "420px",
            }}
          >
            <CardContent>
              <Container>
                <List>
                  {props.screens?.length !== 0 ? (
                    props.screens?.map((screen) => (
                      <ListItem
                        key={screen.id}
                        secondaryAction={
                          <Stack direction="row" spacing={2}>
                            <IconButton
                              edge="end"
                              onClick={(ev) => props.onEdit(screen.id)}
                            >
                              <EditIcon />
                            </IconButton>
                            <IconButton
                              edge="end"
                              onClick={(ev) => props.onDelete(screen.name)}
                            >
                              <DeleteIcon />
                            </IconButton>
                          </Stack>
                        }
                      >
                        <ListItemText primary={screen.name} />
                      </ListItem>
                    ))
                  ) : (
                    <Typography>No screens found.</Typography>
                  )}
                </List>
              </Container>
            </CardContent>
          </SimpleBar>
        </Card>
      </Wrapper>
    </div>
  );
};
