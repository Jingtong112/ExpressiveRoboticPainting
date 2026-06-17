FROM python:3.10-slim

WORKDIR /app

COPY Python/requirements.txt Python/requirements.txt
RUN pip install --no-cache-dir -r Python/requirements.txt

COPY Python Python
COPY Data Data
COPY Scripts Scripts

CMD ["bash"]
