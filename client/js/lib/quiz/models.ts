import { Task } from "../task/models";

export type QuizId = string;

export class QuizInfo {
    public quizId: QuizId;
}

export class Quiz {
    public quizId: QuizId;
    public tasks: Task[];
}
