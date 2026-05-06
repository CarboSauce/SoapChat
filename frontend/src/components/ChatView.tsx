import { useState, useEffect, useRef } from 'react'
import { useQuery, useMutation } from '@apollo/client'
import { GET_MESSAGES, CREATE_MESSAGE } from '../graphql'
import Avatar from './Avatar'
import '../styles/messages.css'

interface ChatViewProps {
  groupId: string
}

export default function ChatView({ groupId }: ChatViewProps) {
  const [messageText, setMessageText] = useState('')
  const [error, setError] = useState('')
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const LIMIT = 50

  const { data, loading, error: queryError, refetch } = useQuery(GET_MESSAGES, {
    variables: {
      groupId,
      skip: 0,
      limit: LIMIT
    },
    skip: !groupId
  })

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [data?.messages])

  useEffect(() => {
    if (!groupId || !data?.messages?.length) return

    const interval = setInterval(() => {
      refetch()
    }, 3000)

    return () => clearInterval(interval)
  }, [groupId, data?.messages, refetch])

  const [createMessage, { loading: sendLoading }] = useMutation(CREATE_MESSAGE, {
    onCompleted: () => {
      setMessageText('')
      refetch()
    },
    onError: (err) => {
      setError(err.message)
    }
  })

  const handleSendMessage = (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!messageText.trim()) {
      setError('Message cannot be empty')
      return
    }

    createMessage({
      variables: {
        groupId,
        text: messageText
      }
    })
  }

  const messages = data?.messages || []

  return (
    <div className="chat-view">
      <div className="messages-container">
        {queryError && (
          <div className="error-message">{queryError.message}</div>
        )}

        {loading && <div className="loading">Loading messages...</div>}

        {messages.length === 0 && !loading && (
          <div className="no-messages">No messages yet</div>
        )}

        {messages.map((msg: any, index: number) => (
          <div key={msg.id || index} className="message">
            <div className="message-content">
              <Avatar userId={msg.sender?.id || ''} size="small" />
              <div className="message-body">
                <div className="message-header">
                  <span className="message-sender">{msg.sender?.name || 'Unknown'}</span>
                  <span className="message-time">
                    {new Date(msg.sentAt).toLocaleTimeString()}
                  </span>
                </div>
                <div className="message-text">{msg.text}</div>
              </div>
            </div>
          </div>
        ))}

        <div ref={messagesEndRef} />
      </div>

      <form onSubmit={handleSendMessage} className="message-input-form">
        {error && <div className="error-message">{error}</div>}
        <div className="input-container">
          <input
            type="text"
            value={messageText}
            onChange={(e) => setMessageText(e.target.value)}
            placeholder="Type a message..."
            className="form-input"
            disabled={sendLoading}
          />
          <button
            type="submit"
            className="btn btn-primary"
            disabled={sendLoading}
          >
            {sendLoading ? 'Sending...' : 'Send'}
          </button>
        </div>
      </form>
    </div>
  )
}
