import { FC } from "react";
import { useParams } from "react-router";
import { useNuiRequest } from "fivem-nui-react-lib";
import { useAppSelector } from "../hooks/store";
import { Card, CardContent, Container, Typography } from "@mui/material";
import { ScheduleForm } from "../components/ScheduleForm";
import styled from "styled-components";
import SimpleBar from "simplebar-react";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;
export const EditSchedulePage: FC = () => {
  const { scheduleId } = useParams();

  const { send } = useNuiRequest();

  const schedule = useAppSelector((state) =>
    state.schedules.schedules?.find((s) => s.id === parseInt(scheduleId!))
  );

  const screens = useAppSelector((state) => state.screens.screens);

  const onSubmit = (data: any) => {
    data.endDate = data.endDate || "";

    data.screen = screens?.find((s) => s.name === data.screenName);

    send("editSchedule", { payload: JSON.stringify(data) }).then(() => {});
  };

  return (
    <Container>
      <Typography sx={{ marginTop: "50px", marginBottom: "10px" }} variant="h5">
        Edit Schedule
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
                schedule={schedule}
                screens={screens}
                onSubmitHandler={onSubmit}
              />
            </CardContent>
          </SimpleBar>
        </Card>
      </Wrapper>
    </Container>
  );
};
