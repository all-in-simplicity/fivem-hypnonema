import { FC } from "react";
import { Card, CardContent, Container, Typography } from "@mui/material";
import styled from "styled-components";
import SimpleBar from "simplebar-react";
import { ScheduleForm } from "../components/ScheduleForm";
import { useAppSelector } from "../hooks/store";
import { useNuiRequest } from "fivem-nui-react-lib";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;

export const CreateSchedulePage: FC = () => {
  const screens = useAppSelector((state) => state.screens.screens);

  const { send } = useNuiRequest();

  const onSubmitHandler = (data: any) => {
    data.endDate = data.endDate || "";
    data.screen = screens?.find((s) => s.name === data.screenName);

    send("createSchedule", { payload: JSON.stringify(data) }).then(() => {});
  };

  return (
    <Container>
      <Typography sx={{ marginTop: "50px", marginBottom: "10px" }} variant="h5">
        Create Schedule
      </Typography>
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
              maxHeight: "430px",
            }}
          >
            <CardContent>
              <ScheduleForm
                onSubmitHandler={onSubmitHandler}
                schedule={null}
                screens={screens}
              />
            </CardContent>
          </SimpleBar>
        </Card>
      </Wrapper>
    </Container>
  );
};
